namespace ExpenseSplitter.Domain.Settlements;

public interface ISettlementRepository
{
    Task<Settlement?> GetByIdAsync(SettlementId id, CancellationToken cancellationToken = default);
}
