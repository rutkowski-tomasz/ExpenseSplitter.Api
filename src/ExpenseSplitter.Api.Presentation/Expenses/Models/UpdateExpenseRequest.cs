namespace ExpenseSplitter.Api.Presentation.Expenses.Models;

public sealed record UpdateExpenseRequest(
    string Title,
    DateOnly PaymentDate,
    Guid PayingParticipantId,
    IEnumerable<UpdateExpenseRequestAllocation> Allocations
);

public sealed record UpdateExpenseRequestAllocation(
    Guid? Id,
    Guid ParticipantId,
    decimal Value
);
