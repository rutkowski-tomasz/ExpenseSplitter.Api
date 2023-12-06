using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.JoinSettlement;

public class JoinSettlementCommandHandler : ICommandHandler<JoinSettlementCommand, Guid>
{
    private readonly ISettlementRepository settlementRepository;
    private readonly ISettlementUserRepository settlementUserRepository;
    private readonly IUserContext userContext;
    private readonly IUnitOfWork unitOfWork;

    public JoinSettlementCommandHandler(
        ISettlementRepository settlementRepository,
        ISettlementUserRepository settlementUserRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork
    )
    {
        this.settlementRepository = settlementRepository;
        this.settlementUserRepository = settlementUserRepository;
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

        if (await settlementUserRepository.CanUserAccessSettlement(settlement.Id, cancellationToken))
        {
            return Result.Failure<Guid>(SettlementUserErrors.AlreadyJoined);
        }

        var settlementUserResult = SettlementUser.Create(settlement.Id, userContext.UserId);
        if (settlementUserResult.IsFailure)
        {
            return Result.Failure<Guid>(settlementUserResult.Error);
        }

        var settlementUser = settlementUserResult.Value;
        settlementUserRepository.Add(settlementUser);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(settlement.Id.Value);
    }
}