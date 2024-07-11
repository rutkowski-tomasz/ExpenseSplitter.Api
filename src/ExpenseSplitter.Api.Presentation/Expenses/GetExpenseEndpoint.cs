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

public class GetExpenseEndpoint() : Endpoint<Guid, GetExpenseQuery, GetExpenseQueryResult, GetExpenseResponse>(
    Route: "{expenseId}",
    Group: EndpointGroup.Expenses,
    Method: EndpointMethod.Post,
    MapRequest: request => new (request),
    MapResponse: result => new (
        result.Id,
        result.Title,
        result.PayingParticipantId,
        result.PaymentDate,
        result.Amount,
        result.Allocations.Select(x => new GetExpenseResponseAllocation(
            x.Id,
            x.ParticipantId,
            x.Amount
        ))
    ),
    ErrorStatusCodes: [
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ]
);
