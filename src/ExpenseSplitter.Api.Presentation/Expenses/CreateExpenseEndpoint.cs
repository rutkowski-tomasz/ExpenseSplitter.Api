using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public record CreateExpenseRequest(
    string Name,
    DateOnly PaymentDate,
    Guid SettlementId,
    Guid PayingParticipantId,
    IEnumerable<CreateExpenseRequestAllocation> Allocations
);

public sealed record CreateExpenseRequestAllocation(
    Guid ParticipantId,
    decimal Value
);

public class CreateExpenseEndpoint() : Endpoint<CreateExpenseRequest, CreateExpenseCommand, Guid, Guid>(
    Route: "",
    Group: EndpointGroup.Expenses,
    Method: EndpointMethod.Post,
    MapRequest: request => new (
        request.Name,
        request.PaymentDate,
        request.SettlementId,
        request.PayingParticipantId,
        request.Allocations.Select(x => new CreateExpenseCommandAllocation(
            x.ParticipantId,
            x.Value
        ))
    ),
    MapResponse: result => result,
    ErrorStatusCodes: [
        StatusCodes.Status400BadRequest,
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ]
);
