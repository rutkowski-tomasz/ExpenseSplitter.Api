using ExpenseSplitter.Domain.Expenses;
using ExpenseSplitter.Domain.Users;

namespace ExpenseSplitter.Domain.Settlements;

public class Settlement
{
    private Settlement()
    {

    }

	public SettlementId Id { get; private set; }

	public string Name { get; private set; } = string.Empty;

    private readonly HashSet<Participant> participants = new();

    private readonly HashSet<Expense> expenses = new();

    public static Settlement Create(string name)
    {
        var settlement = new Settlement()
        {
            Id = new SettlementId(Guid.NewGuid()),
            Name = name,
        };

        return settlement;
    }

    public void AddParticipant(UserId userId, string nickname)
    {
        if (participants.Any(x => x.UserId == userId))
        {
            throw new InvalidOperationException($"User with ID {userId} is already added");
        }

        var participant = Participant.Create(Id, userId, nickname);

        participants.Add(participant);
    }

    public void AddExpense(string name, ParticipantId payingParticipantId, string currency)
    {
        var expense = Expense.Create(Id, name, payingParticipantId, currency);

        expenses.Add(expense);
    }
}
