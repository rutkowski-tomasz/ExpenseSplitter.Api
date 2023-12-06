using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Domain.SettlementUsers;

public interface ISettlementUserRepository
{
    void Add(SettlementUser settlementUser);

    Task<bool> CanUserAccessSettlement(SettlementId settlementId, CancellationToken cancellationToken);
}
