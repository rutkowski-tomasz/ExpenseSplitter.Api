namespace ExpenseSplitter.Api.Application.Expenses.GetExpense;

public sealed record GetExpenseResponse(
    Guid Id,
    string Title,
    Guid PayingParticipantId,
    DateTime PaymentDate,
    decimal Amount,
    IEnumerable<GetExpenseResponseAllocation> Allocations
);

public sealed record GetExpenseResponseAllocation(
    Guid Id,
    Guid ParticipantId,
    decimal Amount
);
