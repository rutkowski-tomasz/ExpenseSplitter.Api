using ExpenseSplitter.Domain.Expenses;

namespace ExpenseSplitter.Domain.Settlements;

public class Settlement
{
	public SettlementId Id { get; private set; }

	public string Name { get; private set; } = string.Empty;

    private readonly HashSet<Participant> participants = new();

    private readonly HashSet<Expense> expenses = new();
}
