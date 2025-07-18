namespace ExpenseSplitter.Api.Application.Expenses.GetExpense;

public sealed record GetExpenseQueryResult(
    Guid Id,
    string Title,
    Guid PayingParticipantId,
    DateOnly PaymentDate,
    decimal Amount,
    IEnumerable<GetExpenseQueryResultAllocation> Allocations
);

public sealed record GetExpenseQueryResultAllocation(
    Guid Id,
    Guid ParticipantId,
    decimal Amount
);
