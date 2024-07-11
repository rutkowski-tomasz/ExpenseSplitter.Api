using ExpenseSplitter.Api.Application.Expenses.GetExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public sealed record GetExpenseResponse(
    Guid Id,
    string Title,
    Guid PayingParticipantId,
    DateOnly PaymentDate,
    decimal Amount,
    IEnumerable<GetExpenseResponseAllocation> Allocations
);

public sealed record GetExpenseResponseAllocation(
    Guid Id,
    Guid ParticipantId,
    decimal Amount
);


public class GetExpenseEndpoint : Endpoint<Guid, GetExpenseQuery, GetExpenseQueryResult, GetExpenseResponse>
{
    public override GetExpenseQuery MapRequest(Guid source)
    {
        return new GetExpenseQuery(source);
    }

    public override GetExpenseResponse MapResponse(GetExpenseQueryResult source)
    {
        return new GetExpenseResponse(
            source.Id,
            source.Title,
            source.PayingParticipantId,
            source.PaymentDate,
            source.Amount,
            source.Allocations.Select(x => new GetExpenseResponseAllocation(
                x.Id,
                x.ParticipantId,
                x.Amount
            ))
        );
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Expenses()
            .MapGet("{expenseId}", (Guid expenseId, ISender sender) => Handle(expenseId, sender))
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
