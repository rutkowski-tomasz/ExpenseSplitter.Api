using ExpenseSplitter.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Repositories;

internal sealed class SettlementRepository : Repository<Settlement, SettlementId>, ISettlementRepository
{
    public SettlementRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Settlement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Settlement>()
            .ToListAsync(cancellationToken);
    }
}
