using ExpenseSplitter.Api.Application.Expenses.UpdateExpense;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
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
    Endpoints.Expenses.Put("{expenseId}").ProducesErrorCodes([
        StatusCodes.Status400BadRequest,
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ]),
    request => new UpdateExpenseCommand(
        request.ExpenseId,
        request.Body.Title,
        request.Body.PaymentDate,
        request.Body.PayingParticipantId,
        request.Body.Allocations.Select(x => new UpdateExpenseCommandAllocation(
            x.Id,
            x.ParticipantId,
            x.Value
        ))
    )
);
