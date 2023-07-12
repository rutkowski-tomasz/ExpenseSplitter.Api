using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Domain.Expenses;

public class ExpenseAllocationParticipant
{
	public ExpenseAllocationId ExpenseAllocationId { get; private set; }

	public ParticipantId ParticipantId { get; private set; }

	public decimal Part { get; private set; }
}
