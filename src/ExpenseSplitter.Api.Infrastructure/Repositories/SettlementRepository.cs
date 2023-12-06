using System.Security.Cryptography.X509Certificates;
using ExpenseSplitter.Api.Domain.Settlements;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

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

    public async Task<Settlement?> GetSettlementByInviteCode(string inviteCode, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<Settlement>()
            .SingleOrDefaultAsync(x => x.InviteCode == inviteCode, cancellationToken);
    }

    public async Task<bool> RemoveSettlementById(SettlementId settlementId, CancellationToken cancellationToken = default)
    {
        var settlement = await DbContext
            .Set<Settlement>()
            .FirstOrDefaultAsync(x => x.Id == settlementId, cancellationToken);

        if (settlement is null)
        {
            return false;
        }

        DbContext.Set<Settlement>().Remove(settlement);
        return true;
    }
}
