using ExpenseSplitter.Domain.Participants;
using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Expenses;

class Expense
{
    private Expense()
    {

    }

    public ExpenseId Id { get; private set; }

    public SettlementId SettlementId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public ParticipantId PayingParticipantId { get; set; }

    public string Currency { get; private set; } = string.Empty;

    public static Expense Create(
        SettlementId settlementId,
        string name,
        ParticipantId payingParticipantId,
        string currency
    )
    {
        var expense = new Expense()
        {
            SettlementId = settlementId,
            Id = new ExpenseId(Guid.NewGuid()),
            PayingParticipantId = payingParticipantId,
            Name = name,
            Currency = currency,
        };

        return expense;
    }
}
