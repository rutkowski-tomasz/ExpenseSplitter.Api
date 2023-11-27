using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
