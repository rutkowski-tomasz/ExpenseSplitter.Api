using ExpenseSplitter.Api.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User?> GetByIdentityId(string identityId, CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<User>()
            .SingleAsync(x => x.IdentityId == identityId, cancellationToken: cancellationToken);
    }
}
