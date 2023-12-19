namespace ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;

public sealed record GetExpensesForSettlementQueryResult(
    IEnumerable<GetExpensesForSettlementQueryResultExpense> Expenses
);

public sealed record GetExpensesForSettlementQueryResultExpense(
    Guid Id,
    string Title,
    decimal Amount
);
