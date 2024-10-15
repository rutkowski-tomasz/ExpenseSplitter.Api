using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Expenses;

public static class ExpenseErrors
{
    public static readonly AppError EmptyTitle = new(
        ErrorType.Validation,
        "Can't create expense with empty name"
    );

    public static readonly AppError NotFound = new(
        ErrorType.NotFound,
        "The expense with the specified identifier was not found"
    );
}
