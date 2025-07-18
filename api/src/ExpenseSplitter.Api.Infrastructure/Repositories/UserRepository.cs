using ExpenseSplitter.Api.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class UserRepository(ApplicationDbContext dbContext)
    : Repository<User, UserId>(dbContext), IUserRepository
{
    public Task<bool> Exists(string email, CancellationToken cancellationToken)
    {
        return DbContext
            .Set<User>()
            .AnyAsync(x => x.Email == email, cancellationToken);
    }
}
