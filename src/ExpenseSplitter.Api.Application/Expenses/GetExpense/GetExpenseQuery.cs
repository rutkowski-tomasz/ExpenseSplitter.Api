using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Expenses.GetExpense;

public sealed record GetExpenseQuery(Guid ExpenseId) : IQuery<GetExpenseResponse>;