using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Exceptions;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;

public class UpdateSettlementCommandHandler(
    ISettlementUserRepository userRepository,
    ISettlementRepository repository,
    IParticipantRepository participantRepository,
    IDateTimeProvider timeProvider,
    IUnitOfWork unitOfWork
) : ICommandHandler<UpdateSettlementCommand>
{
    public async Task<Result> Handle(UpdateSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.Id);
        if (!await userRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return SettlementErrors.Forbidden;
        }

        var settlement = await repository.GetById(settlementId, cancellationToken);
        if (settlement is null)
        {
            return SettlementErrors.NotFound;
        }

        settlement.SetUpdatedOnUtc(timeProvider.UtcNow);
        var setNameResult = settlement.SetName(request.Name);
        if (setNameResult.IsFailure)
        {
            return setNameResult;
        }

        var participants = await participantRepository.GetAllBySettlementId(settlementId, cancellationToken);

        RemoveNonExistingParticipants(participants, request);
        var createResult = CreateNewParticipants(request, settlementId);
        if (createResult.IsFailure)
        {
            return createResult;
        }

        var updateResult = UpdateExistingParticipants(participants, request);
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

    private void RemoveNonExistingParticipants(IEnumerable<Participant> participants, UpdateSettlementCommand updateCommand)
    {
        var participantsToRemove = participants
            .Where(x => !updateCommand.Participants.Any(y => y.Id.HasValue && new ParticipantId(y.Id.Value) == x.Id));

        foreach (var participantToRemove in participantsToRemove)
        {
            participantRepository.Remove(participantToRemove);
        }
    }

    private Result CreateNewParticipants(UpdateSettlementCommand updateCommand, SettlementId settlementId)
    {
        var newParticipants = updateCommand
            .Participants
            .Where(x => !x.Id.HasValue)
            .Select(x => Participant.Create(
                settlementId,
                x.Nickname
            ))
            .ToList();
        
        var participantWithFailure = newParticipants.Find(x => x.IsFailure);
        if (participantWithFailure is not null)
        {
            return Result.Failure(participantWithFailure.AppError);
        }
        
        foreach (var newParticipant in newParticipants)
        {
            participantRepository.Add(newParticipant.Value);
        }

        return Result.Success();
    }

    private static Result UpdateExistingParticipants(IEnumerable<Participant> participants, UpdateSettlementCommand updateCommand)
    {
        var updates = updateCommand
            .Participants
            .Where(x => x.Id.HasValue)
            .Select(x => new {
                UpdateModel = x,
                Entity = participants.SingleOrDefault(y => y.Id == new ParticipantId(x.Id!.Value))
            })
            .Where(x => x.Entity is not null);

        foreach (var update in updates)
        {
            var setNicknameResult = update.Entity!.SetNickname(update.UpdateModel.Nickname);
            if (setNicknameResult.IsFailure)
            {
                return setNicknameResult;
            }
        }

        return Result.Success();
    }
}
