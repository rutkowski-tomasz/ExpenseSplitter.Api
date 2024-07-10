using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

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


public class CreateExpenseEndpoint : IEndpoint,
    IMapper<CreateExpenseRequest, CreateExpenseCommand>
{
    public CreateExpenseCommand Map(CreateExpenseRequest source)
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

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Expenses()
            .MapPost("", (
                CreateExpenseRequest request,
                IHandler<
                    CreateExpenseRequest,
                    CreateExpenseCommand,
                    Guid,
                    Guid
                > handler) => handler.Handle(request)
            )
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
