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

    public static Result<User> Create(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            return Result.Failure<User>(UserErrors.EmptyNickname);
        }

        var user = new User(UserId.New(), nickname);

        return user;
    }
}
