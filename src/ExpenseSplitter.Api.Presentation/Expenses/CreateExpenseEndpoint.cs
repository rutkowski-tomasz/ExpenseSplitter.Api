using ExpenseSplitter.Api.Application.Expenses.CreateExpense;
using ExpenseSplitter.Api.Presentation.Extensions;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using ExpenseSplitter.Api.Presentation.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Expenses;

public record CreateExpenseRequest([FromBody] CreateExpenseRequestBody Body);

public record CreateExpenseRequestBody(
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
    Endpoints.Expenses.Post("").ProducesErrorCodes(
        StatusCodes.Status400BadRequest,
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ),
    request => new CreateExpenseCommand(
        request.Body.Name,
        request.Body.PaymentDate,
        request.Body.SettlementId,
        request.Body.PayingParticipantId,
        request.Body.Allocations.Select(x => new CreateExpenseCommandAllocation(
            x.ParticipantId,
            x.Value
        ))
    ),
    result => result,
    builder => builder
        .RequireRateLimiting(RateLimitingExtensions.IpRateLimiting)
        .AddEndpointFilter<IdempotentFilter>()
);
