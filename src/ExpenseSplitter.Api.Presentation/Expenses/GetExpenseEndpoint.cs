using ExpenseSplitter.Api.Application.Expenses.GetExpense;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

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


public class GetExpenseEndpoint : IEndpoint,
    IMapper<Guid, GetExpenseQuery>,
    IMapper<GetExpenseQueryResult, GetExpenseResponse>
{
    public GetExpenseQuery Map(Guid source)
    {
        return new GetExpenseQuery(source);
    }

    public GetExpenseResponse Map(GetExpenseQueryResult source)
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

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Expenses()
            .MapGet("{expenseId}", (
                Guid expenseId,
                IHandler<
                    Guid,
                    GetExpenseQuery,
                    GetExpenseQueryResult,
                    GetExpenseResponse
                > handler) => handler.Handle(expenseId)
            )
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
