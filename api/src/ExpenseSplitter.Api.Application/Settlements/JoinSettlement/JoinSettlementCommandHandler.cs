using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.Settlements.JoinSettlement;

public class JoinSettlementCommandHandler(
    ISettlementRepository settlementRepository,
    ISettlementUserRepository settlementUserRepository,
    IUserContext userContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<JoinSettlementCommand, Guid>
{
    public async Task<Result<Guid>> Handle(JoinSettlementCommand request, CancellationToken cancellationToken)
    {
        var settlement = await settlementRepository.GetByInviteCode(request.InviteCode, cancellationToken);
        if (settlement is null)
        {
            return SettlementErrors.NotFound;
        }

        if (await settlementUserRepository.CanUserAccessSettlement(settlement.Id, cancellationToken))
        {
            return SettlementUserErrors.AlreadyJoined;
        }

        var settlementUser = SettlementUser.Create(settlement.Id, userContext.UserId);
        settlementUserRepository.Add(settlementUser);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(settlement.Id.Value);
    }
}
