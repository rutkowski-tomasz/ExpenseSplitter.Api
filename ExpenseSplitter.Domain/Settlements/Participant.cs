using ExpenseSplitter.Domain.Expenses;
using ExpenseSplitter.Domain.Users;

namespace ExpenseSplitter.Domain.Settlements;

public class Participant
{
    public ParticipantId ParticipantId { get; set; }

    public UserId UserId { get; private set; }

    public SettlementId SettlementId { get; private set; }

    public string Nickname { get; private set; } = string.Empty;

    private readonly HashSet<ExpensePartParticipant> expensePartParticipants = new();
}
