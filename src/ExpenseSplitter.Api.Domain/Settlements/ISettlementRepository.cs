namespace ExpenseSplitter.Api.Domain.Settlements;

public interface ISettlementRepository
{
    Task<Settlement?> GetByIdAsync(SettlementId id, CancellationToken cancellationToken = default);

    void Add(Settlement settlement);

    Task<IEnumerable<Settlement>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<Settlement?> GetSettlementByInviteCode(string inviteCode, CancellationToken cancellationToken = default);

    void Remove(Settlement settlement);
}
