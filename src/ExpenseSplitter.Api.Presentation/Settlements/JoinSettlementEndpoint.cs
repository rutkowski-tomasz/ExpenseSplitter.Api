using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record JoinSettlementRequest(string InviteCode);

public class CreateExpenseEndpoint() : Endpoint<JoinSettlementRequest, JoinSettlementCommand>(
    Route: "join",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Post,
    MapRequest: request => new (request.InviteCode),
    ErrorStatusCodes: [
        StatusCodes.Status400BadRequest,
        StatusCodes.Status404NotFound
    ]
);
