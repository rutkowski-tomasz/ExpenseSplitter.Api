using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Exceptions;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;

public class UpdateSettlementCommandHandler(
    ISettlementUserRepository settlementUserRepository,
    ISettlementRepository repository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateSettlementCommand>
{
    public async Task<Result> Handle(UpdateSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.Id);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return SettlementErrors.Forbidden;
        }

        var settlement = await repository.GetById(settlementId, cancellationToken);
        if (settlement is null)
        {
            return SettlementErrors.NotFound;
        }

        settlement.SetUpdatedOnUtc(dateTimeProvider.UtcNow);
        var setNameResult = settlement.SetName(request.Name);
        if (setNameResult.IsFailure)
        {
            return setNameResult;
        }

        var removeResult = RemoveNonExistingParticipants(settlement, request);
        if (removeResult.IsFailure)
        {
            return removeResult;
        }

        var createResult = CreateNewParticipants(settlement, request);
        if (createResult.IsFailure)
        {
            return createResult;
        }

        var updateResult = UpdateExistingParticipants(settlement, request);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Failure(ConcurrencyException.ConcurrencyAppError);
        }

        return Result.Success();
    }

    private static Result RemoveNonExistingParticipants(Settlement settlement, UpdateSettlementCommand updateCommand)
    {
        var participantIds = updateCommand.Participants
            .Where(x => x.Id.HasValue)
            .Select(x => new ParticipantId(x.Id!.Value));

        var participantsToRemove = settlement.Participants
            .Where(x => !participantIds.Contains(x.Id))
            .ToList();

        foreach (var participantToRemove in participantsToRemove)
        {
            var removeAllocationResult = settlement.RemoveParticipant(participantToRemove.Id);
            if (removeAllocationResult.IsFailure)
            {
                return removeAllocationResult;
            }
        }

        return Result.Success();
    }

    private static Result CreateNewParticipants(Settlement settlement, UpdateSettlementCommand updateCommand)
    {
        var newParticipants = updateCommand.Participants
            .Where(x => !x.Id.HasValue)
            .ToList();
        
        foreach (var newParticipant in newParticipants)
        {
            var addResult = settlement.AddParticipant(newParticipant.Nickname);
            if (addResult.IsFailure)
            {
                return addResult;
            }
        }

        return Result.Success();
    }

    private static Result UpdateExistingParticipants(Settlement settlement, UpdateSettlementCommand updateCommand)
    {
        var updateModels = updateCommand.Participants
            .Where(x => x.Id.HasValue)
            .ToList();
        
        foreach (var updateModel in updateModels)
        {
            var participantId = new ParticipantId(updateModel.Id!.Value);
            var participant = settlement.Participants.FirstOrDefault(y => y.Id == participantId);
            if (participant is null)
            {
                return SettlementErrors.ParticipantNotFound;
            }

            var updateResult = settlement.UpdateParticipant(participantId, updateModel.Nickname);
            if (updateResult.IsFailure)
            {
                return updateResult;
            }
        }

        return Result.Success();
    }
}
