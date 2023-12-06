using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Expenses.DeleteExpense;

public sealed record DeleteExpenseCommand(
    Guid Id
) : ICommand;
