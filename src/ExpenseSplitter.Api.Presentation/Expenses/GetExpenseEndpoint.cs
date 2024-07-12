using ExpenseSplitter.Api.Application.Expenses.GetExpense;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public record GetExpenseRequest([FromRoute] Guid ExpenseId);

public record GetExpenseResponse(
    Guid Id,
    string Title,
    Guid PayingParticipantId,
    DateOnly PaymentDate,
    decimal Amount,
    IEnumerable<GetExpenseResponseAllocation> Allocations
);

public record GetExpenseResponseAllocation(
    Guid Id,
    Guid ParticipantId,
    decimal Amount
);

public class GetExpenseEndpoint() : Endpoint<GetExpenseRequest, GetExpenseQuery, GetExpenseQueryResult, GetExpenseResponse>(
    Endpoints.Expenses.Get("{expenseId}").ProducesErrorCodes(
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ),
    request => new (request.ExpenseId),
    result => new (
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
    )
);
