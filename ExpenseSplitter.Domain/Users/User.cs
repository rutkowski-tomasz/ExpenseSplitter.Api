namespace ExpenseSplitter.Domain.Users;

public class User
{
    public UserId Id { get; private set; }

    public string Nickname { get; private set; } = string.Empty;

    public static User Create(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            throw new ArgumentException($"{nameof(nickname)} can't be empty or whitespace");
        }
        
        var user = new User()
        {
            Id = new UserId(Guid.NewGuid()),
            Nickname = nickname,
        };

        return user;
    }
}
