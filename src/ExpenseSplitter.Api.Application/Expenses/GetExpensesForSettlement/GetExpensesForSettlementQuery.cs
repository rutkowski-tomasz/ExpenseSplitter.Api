using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;

public sealed record GetExpensesForSettlementQuery(
    Guid SettlementId
) : ICachedQuery<GetExpensesForSettlementQueryResult>
{
    public string Key => $"expenses-for-{SettlementId}";

    public TimeSpan? Expiration => null;
}
