namespace ExpenseSplitter.Api.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);

    Task<User?> GetByIdentityId(string identityId, CancellationToken cancellationToken = default);
    
    void Add(User user);
}
