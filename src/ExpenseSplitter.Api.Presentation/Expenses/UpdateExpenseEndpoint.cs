using ExpenseSplitter.Api.Application.Expenses.UpdateExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
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

public class UpdateExpenseEndpoint : IEndpoint,
    IMapper<UpdateExpenseRequest, UpdateExpenseCommand>
{
    public UpdateExpenseCommand Map(UpdateExpenseRequest source)
    {
        return new UpdateExpenseCommand(
            source.ExpenseId,
            source.Body.Title,
            source.Body.PaymentDate,
            source.Body.PayingParticipantId,
            source.Body.Allocations.Select(x => new UpdateExpenseCommandAllocation(
                x.Id,
                x.ParticipantId,
                x.Value
            ))
        );
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Expenses()
            .MapPut("{expenseId}", (
                [AsParameters] UpdateExpenseRequest request,
                IHandlerEmptyResponse<
                    UpdateExpenseRequest,
                    UpdateExpenseCommand
                > handler) => handler.Handle(request)
            )
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
