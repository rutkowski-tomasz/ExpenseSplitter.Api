using ExpenseSplitter.Api.Application.Abstractions.Cqrs;

namespace ExpenseSplitter.Api.Application.Expenses.UpdateExpense;

public sealed record UpdateExpenseCommand(
    Guid Id,
    string Title,
    DateOnly PaymentDate,
    Guid PayingParticipantId,
    IEnumerable<UpdateExpenseCommandAllocation> Allocations
) : ICommand;

public sealed record UpdateExpenseCommandAllocation(
    Guid? Id,
    Guid ParticipantId,
    decimal Value
);
