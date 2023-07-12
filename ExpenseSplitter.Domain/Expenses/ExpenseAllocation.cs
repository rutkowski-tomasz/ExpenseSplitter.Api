namespace ExpenseSplitter.Domain.Expenses;

public class ExpenseAllocation
{
    public ExpenseAllocationId Id { get; private set; }

    public ExpenseId ExpenseId { get; private set; }

    public decimal Value { get; private set; }

    private readonly HashSet<ExpenseAllocationParticipant> expenseAllocationParticipants = new();
}
