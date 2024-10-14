using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;

public sealed record GetExpensesForSettlementQuery(
    Guid SettlementId
) : IQuery<GetExpensesForSettlementQueryResult>;
