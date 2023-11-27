using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Users;

public sealed class User : Entity<UserId>
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
