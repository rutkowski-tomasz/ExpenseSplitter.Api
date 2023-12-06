using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class SettlementUserRepository : Repository<SettlementUser, SettlementUserId>, ISettlementUserRepository
{
    private readonly IUserContext userContext;

    public SettlementUserRepository(
        ApplicationDbContext dbContext,
        IUserContext userContext
    ) : base(dbContext)
    {
        this.userContext = userContext;
    }

    public async Task<bool> CanUserAccessSettlement(SettlementId settlementId, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<SettlementUser>()
            .AnyAsync(x => x.SettlementId == settlementId && x.UserId == userContext.UserId, cancellationToken);
    }
}
