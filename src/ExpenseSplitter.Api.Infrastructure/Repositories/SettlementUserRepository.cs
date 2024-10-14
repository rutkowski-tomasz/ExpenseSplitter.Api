using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class SettlementUserRepository(
    ApplicationDbContext dbContext,
    IUserContext context
) : Repository<SettlementUser, SettlementUserId>(dbContext), ISettlementUserRepository
{
    public Task<bool> CanUserAccessSettlement(SettlementId settlementId, CancellationToken cancellationToken)
    {
        return DbContext
            .Set<SettlementUser>()
            .AnyAsync(x => x.SettlementId == settlementId && x.UserId == context.UserId, cancellationToken);
    }

    public Task<SettlementUser?> GetBySettlementId(SettlementId settlementId, CancellationToken cancellationToken)
    {
        return DbContext
            .Set<SettlementUser>()
            .FirstOrDefaultAsync(x => x.SettlementId == settlementId && x.UserId == context.UserId, cancellationToken);
    }
}
