using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Application.Abstractions.Idempotency;

namespace ExpenseSplitter.Api.Application.Expenses.DeleteExpense;

public sealed record DeleteExpenseCommand(
    Guid Id
) : ICommand;
