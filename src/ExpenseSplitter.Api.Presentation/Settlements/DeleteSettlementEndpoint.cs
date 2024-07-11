using ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class DeleteSettlementEndpoint() : Endpoint<Guid, DeleteSettlementCommand>(
    Route: "{settlementId}",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Delete,
    MapRequest: request => new (request),
    ErrorStatusCodes: [
        StatusCodes.Status403Forbidden,
        StatusCodes.Status412PreconditionFailed,
        StatusCodes.Status404NotFound
    ]
);
