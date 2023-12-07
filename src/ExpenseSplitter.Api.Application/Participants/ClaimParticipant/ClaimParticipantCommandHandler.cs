using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Participants.ClaimParticipant;

internal sealed class ClaimParticipantCommandHandler : ICommandHandler<ClaimParticipantCommand>
{
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly IParticipantRepository participantRepository;
    private readonly IUnitOfWork unitOfWork;

    public ClaimParticipantCommandHandler(
        ISettlementUserRepository settlementUserRepository,
        IParticipantRepository participantRepository,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementUserRepository = settlementUserRepository;
        this.participantRepository = participantRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ClaimParticipantCommand request, CancellationToken cancellationToken)
    {
        var settlementId = new SettlementId(request.SettlementId);
        var participantId = new ParticipantId(request.ParticipantId);

        var settlementUser = await settlementUserRepository.GetSettlementUserWithSettlementId(settlementId, cancellationToken);
        if (settlementUser is null)
        {
            return Result.Failure(SettlementErrors.Forbidden);
        }

        if (!await participantRepository.IsParticipantInSettlement(settlementId, participantId, cancellationToken))
        {
            return Result.Failure(ParticipantErrors.NotFound);
        }

        settlementUser.SetParticipantId(participantId);
        await unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
