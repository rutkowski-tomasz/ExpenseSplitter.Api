using ExpenseSplitter.Api.Domain.Expenses;

namespace ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;

public sealed record GetExpensesForSettlementQueryResult(
    IEnumerable<GetExpensesForSettlementQueryResultExpense> Expenses
);

public sealed record GetExpensesForSettlementQueryResultExpense(
    ExpenseId Id,
    string Title
);
