using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Users;

public class User
{
    public UserId Id { get; private set; }

    public string Nickname { get; private set; } = string.Empty;

    private readonly HashSet<Participant> participants = new();
}
