namespace ExpenseSplitter.Api.Domain.Users;

public interface IUserRepository
{
    void Add(User user);
    Task<User?> GetById(UserId id, CancellationToken cancellationToken);
    Task<bool> Exists(string email, CancellationToken cancellationToken);
}
