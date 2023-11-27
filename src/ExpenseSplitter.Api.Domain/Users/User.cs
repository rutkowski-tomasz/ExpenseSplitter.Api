using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Users;

public sealed class User : Entity<UserId>
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
    public string IdentityId { get; private set; } = string.Empty;

    public static Result<User> Create(string nickname, string email)
    {
        if (string.IsNullOrWhiteSpace(nickname))
        {
            return Result.Failure<User>(UserErrors.EmptyNickname);
        }

        var user = new User(UserId.New(), nickname, email);

        return user;
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}
