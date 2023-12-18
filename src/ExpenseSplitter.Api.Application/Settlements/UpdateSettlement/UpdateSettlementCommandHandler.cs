using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;

public class UpdateSettlementCommandHandler : ICommandHandler<UpdateSettlementCommand>
{
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly ISettlementRepository settlementRepository;
    private readonly IParticipantRepository participantRepository;
    private readonly IUnitOfWork unitOfWork;

    public UpdateSettlementCommandHandler(
        ISettlementUserRepository settlementUserRepository,
        ISettlementRepository settlementRepository,
        IParticipantRepository participantRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementUserRepository = settlementUserRepository;
        this.settlementRepository = settlementRepository;
        this.participantRepository = participantRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.Id);
        if (!await settlementUserRepository.CanUserAccessSettlement(settlementId, cancellationToken))
        {
            return Result.Failure(SettlementErrors.Forbidden);
        }

        var settlement = await settlementRepository.GetById(settlementId, cancellationToken);
        if (settlement is null)
        {
            return Result.Failure(SettlementErrors.NotFound);
        }

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

        await unitOfWork.SaveChangesAsync(cancellationToken);
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
            ));
        
        var participantWithFailure = newParticipants.FirstOrDefault(x => x.IsFailure);
        if (participantWithFailure is not null)
        {
            return Result.Failure(participantWithFailure.Error);
        }
        
        foreach (var newParticipant in newParticipants)
        {
            participantRepository.Add(newParticipant.Value);
        }

        return Result.Success();
    }

    private Result UpdateExistingParticipants(IEnumerable<Participant> participants, UpdateSettlementCommand updateCommand)
    {
        var updates = updateCommand
            .Participants
            .Where(x => x.Id.HasValue)
            .Select(x => new {
                UpdateModel = x,
                Entity = participants.Single(y => y.Id == new ParticipantId(x.Id!.Value))
            });

        foreach (var update in updates)
        {
            var setNicknameResult = update.Entity.SetNickname(update.UpdateModel.Nickname);
            if (setNicknameResult.IsFailure)
            {
                return setNicknameResult;
            }
        }

        return Result.Success();
    }
}
