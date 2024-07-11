using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

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

public class CreateExpenseEndpoint
    : Endpoint<CreateExpenseRequest, CreateExpenseCommand, Guid, Guid>
{
    public override CreateExpenseCommand MapRequest(CreateExpenseRequest source)
    {
        return new CreateExpenseCommand(
            source.Name,
            source.PaymentDate,
            source.SettlementId,
            source.PayingParticipantId,
            source.Allocations.Select(x => new CreateExpenseCommandAllocation(
                x.ParticipantId,
                x.Value
            ))
        );
    }

    public override Guid MapResponse(Guid result) => result;

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Expenses()
            .MapPost("", (CreateExpenseRequest request, ISender sender)
                => Handle(request, sender))
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
