namespace ExpenseSplitter.Api.Domain.ExpenseAllocations;

public record ExpenseAllocationId(Guid Value)
{
    public static ExpenseAllocationId New() => new(Guid.NewGuid());
}
