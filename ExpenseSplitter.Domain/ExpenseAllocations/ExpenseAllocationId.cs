namespace ExpenseSplitter.Domain.ExpenseAllocations;

public record ExpenseAllocationId(Guid Value)
{
    public static ExpenseAllocationId New() => new(Guid.NewGuid());
}
