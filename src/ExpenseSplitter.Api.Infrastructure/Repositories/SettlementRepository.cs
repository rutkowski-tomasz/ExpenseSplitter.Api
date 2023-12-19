using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class SettlementRepository : Repository<Settlement, SettlementId>, ISettlementRepository
{
    public SettlementRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Settlement>> GetAll(int page, int pageSize, CancellationToken cancellationToken)
    {
        var skip = (page - 1) * pageSize;

        return await DbContext
            .Set<Settlement>()
            .OrderByDescending(x => x.UpdatedOnUtc)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Settlement?> GetByInviteCode(string inviteCode, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<Settlement>()
            .SingleOrDefaultAsync(x => x.InviteCode == inviteCode, cancellationToken);
    }
}
