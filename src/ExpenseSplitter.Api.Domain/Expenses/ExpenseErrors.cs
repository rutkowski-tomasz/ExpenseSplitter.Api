using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Expenses;

public static class ExpenseErrors
{
    public static readonly Error EmptyName = new(
        "Expense.EmptyName",
        "Can't create expense with empty name"
    );

    public static readonly Error NotFound = new(
        "Expense.NotFound",
        "The expense with the specified identifier was not found"
    );
}
