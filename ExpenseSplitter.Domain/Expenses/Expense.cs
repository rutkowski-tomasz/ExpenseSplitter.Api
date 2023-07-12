using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Expenses;

class Expense
{
    public ExpenseId Id { get; private set; }

    public ParticipantId PayingParticipantId { get; set; }

    public string Name { get; private set; } = string.Empty;

    public string Currency { get; private set; } = string.Empty;

    private readonly HashSet<ExpenseAllocation> expenseAllocations = new();
}
