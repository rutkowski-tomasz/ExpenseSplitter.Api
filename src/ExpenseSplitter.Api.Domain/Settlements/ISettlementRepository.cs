namespace ExpenseSplitter.Api.Domain.Settlements;

public interface ISettlementRepository
{
    void Add(Settlement settlement);
    void Remove(Settlement settlement);
    Task<Settlement?> GetById(SettlementId id, CancellationToken cancellationToken);
    Task<IEnumerable<Settlement>> GetAll(int page, int pageSize, CancellationToken cancellationToken);
    Task<Settlement?> GetByInviteCode(string inviteCode, CancellationToken cancellationToken);
}
