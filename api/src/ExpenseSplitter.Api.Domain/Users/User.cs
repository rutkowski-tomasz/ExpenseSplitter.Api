using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private User(
        UserId id,
        string nickname,
        string email
    ) : base(id)
    {
        Nickname = nickname;
        Email = email;
    }
    
    public string Nickname { get; private set; }
    public string Email { get; private set; }

    public static Result<User> Create(string nickname, string email, UserId id)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            return Result.Failure<User>(UserErrors.EmptyNickname);
        }

        var user = new User(id, nickname, email);

        return user;
    }
}
