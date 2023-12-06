using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.Expenses;

public class ExpenseErrors
{
    public static Error EmptyName = new(
        "Expense.EmptyName",
        "Can't create expense with empty name"
    );

    public static Error NonPositiveAmount = new(
        "Expense.NonPositiveAmount",
        "Can't create expense with non positive amount"
    );

    public static Error NotFound = new(
        "Expense.NotFound",
        "The expense with the specified identifier was not found"
    );
}
