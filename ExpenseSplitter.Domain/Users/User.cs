using ExpenseSplitter.Domain.Abstractions;

namespace ExpenseSplitter.Domain.Users;

public class User : Entity<UserId>
{
    private User(
        UserId id,
        string nickname
    ) : base(id)
    {
        Nickname = nickname;
    }
    
    public string Nickname { get; private set; }

    public static User Create(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            throw new ArgumentException($"{nameof(nickname)} can't be empty or whitespace");
        }

        var user = new User(UserId.New(), nickname);

        return user;
    }
}
