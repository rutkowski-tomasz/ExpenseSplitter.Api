using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record JoinSettlementRequest(string InviteCode);

public class CreateExpenseEndpoint() : Endpoint<JoinSettlementRequest, JoinSettlementCommand>(
    Endpoints.Settlements.Post("join").ProducesErrorCodes(
        StatusCodes.Status400BadRequest,
        StatusCodes.Status404NotFound
    ),
    request => new (request.InviteCode)
);
