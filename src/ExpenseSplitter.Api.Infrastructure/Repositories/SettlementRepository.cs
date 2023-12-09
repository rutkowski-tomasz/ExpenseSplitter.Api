using System.Security.Cryptography.X509Certificates;
using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class SettlementRepository : Repository<Settlement, SettlementId>, ISettlementRepository
{
    public SettlementRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Settlement>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var skip = (page - 1) * pageSize;

        return await DbContext
            .Set<Settlement>()
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Settlement?> GetSettlementByInviteCode(string inviteCode, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Settlement>()
            .SingleOrDefaultAsync(x => x.InviteCode == inviteCode, cancellationToken);
    }

    public void Remove(Settlement settlement)
    {
        DbContext
            .Set<Settlement>()
            .Remove(settlement);
    }
}
