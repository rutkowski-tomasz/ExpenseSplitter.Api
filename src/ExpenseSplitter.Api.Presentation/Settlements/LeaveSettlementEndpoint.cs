using ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class LeaveSettlementEndpoint() : Endpoint<Guid, LeaveSettlementCommand>(
    Route: "{settlementId}/leave",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Post,
    MapRequest: request => new (request),
    ErrorStatusCodes: [
        StatusCodes.Status403Forbidden
    ]
);
