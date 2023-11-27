namespace ExpenseSplitter.Api.Domain.Settlements;

public interface ISettlementRepository
{
    Task<Settlement?> GetByIdAsync(SettlementId id, CancellationToken cancellationToken = default);

    void Add(Settlement settlement);

    Task<IEnumerable<Settlement>> GetAllAsync(CancellationToken cancellationToken = default);
}
