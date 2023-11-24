using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Participants;
using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Expenses;

public sealed class Expense : Entity<ExpenseId>
{
    private Expense(
        ExpenseId id,
        SettlementId settlementId,
        string name,
        ParticipantId payingParticipantId
        
    ) : base(id)
    {
        SettlementId = settlementId;
        Name = name;
        PayingParticipantId = payingParticipantId;
    }

    public SettlementId SettlementId { get; private set; }

    public string Name { get; private set; }

    public ParticipantId PayingParticipantId { get; private set; }

    public static Expense Create(
        SettlementId settlementId,
        string name,
        ParticipantId payingParticipantId
    )
    {
        var expense = new Expense(
            ExpenseId.New(),
            settlementId,
            name,
            payingParticipantId
        );

        return expense;
    }
}
