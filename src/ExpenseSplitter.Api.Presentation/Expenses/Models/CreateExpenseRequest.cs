namespace ExpenseSplitter.Api.Presentation.Expenses.Models;

public record CreateExpenseRequest(
    string Name,
    decimal Amount,
    DateTime Date,
    Guid SettlementId,
    Guid PayingParticipantId,
    IEnumerable<CreateExpenseRequestAllocation> Allocations
);

public sealed record CreateExpenseRequestAllocation(
    Guid ParticipantId,
    decimal Value
);
