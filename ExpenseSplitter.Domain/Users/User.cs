using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Users;

public class User
{
    public UserId Id { get; private set; }

    public string Nickname { get; private set; } = string.Empty;

    private readonly HashSet<Participant> participants = new();

    public static User Create(string nickname)
    {
        var user = new User()
        {
            Id = new UserId(Guid.NewGuid()),
            Nickname = nickname,
        };

        return user;
    }
}
