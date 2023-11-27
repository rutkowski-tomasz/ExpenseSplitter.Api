namespace ExpenseSplitter.Api.Domain.Expenses;

public record ExpenseId(Guid Value)
{
    public static ExpenseId New() => new(Guid.NewGuid());
}
