using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.Settlements.JoinSettlement;

public class JoinSettlementCommandHandler : ICommandHandler<JoinSettlementCommand, Guid>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly IParticipantRepository participantRepository;
    private readonly IUserContext userContext;
    private readonly IUnitOfWork unitOfWork;

    public JoinSettlementCommandHandler(
        ISettlementRepository settlementRepository,
        IParticipantRepository participantRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementRepository = settlementRepository;
        this.participantRepository = participantRepository;
        this.userContext = userContext;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(JoinSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlement = await settlementRepository.GetSettlementByInviteCode(request.InviteCode, cancellationToken);

        if (settlement is null)
        {
            return Result.Failure<Guid>(SettlementErrors.NotFound);
        }

        var participantResult = Participant.Create(settlement.Id, request.Nickname);

        if (participantResult.IsFailure)
        {
            return Result.Failure<Guid>(participantResult.Error);
        }

        var participant = participantResult.Value;
        participant.SetUserId(userContext.UserId);
        
        participantRepository.Add(participant);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(settlement.Id.Value);
    }
}