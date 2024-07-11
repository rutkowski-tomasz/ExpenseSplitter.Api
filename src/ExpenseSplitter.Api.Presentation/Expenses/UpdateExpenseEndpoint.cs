using ExpenseSplitter.Api.Application.Expenses.UpdateExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public sealed record UpdateExpenseRequest(
    [FromRoute] Guid ExpenseId,
    [FromBody] UpdateExpenseRequestBody Body
);

public sealed record UpdateExpenseRequestBody(
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

public class UpdateExpenseEndpoint() : Endpoint<UpdateExpenseRequest, UpdateExpenseCommand>(
    Route: "{expenseId}",
    Group: EndpointGroup.Expenses,
    Method: EndpointMethod.Put,
    MapRequest: source => new (
        source.ExpenseId,
        source.Body.Title,
        source.Body.PaymentDate,
        source.Body.PayingParticipantId,
        source.Body.Allocations.Select(x => new UpdateExpenseCommandAllocation(
            x.Id,
            x.ParticipantId,
            x.Value
        ))
    ),
    ErrorStatusCodes: [
        StatusCodes.Status400BadRequest,
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ]
);
