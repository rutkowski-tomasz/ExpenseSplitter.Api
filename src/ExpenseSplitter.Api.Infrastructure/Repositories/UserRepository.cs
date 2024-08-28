using ExpenseSplitter.Api.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> Exists(string email, CancellationToken cancellationToken)
    {
        return await DbContext
            .Set<User>()
            .AnyAsync(x => x.Email == email, cancellationToken);
    }
}
