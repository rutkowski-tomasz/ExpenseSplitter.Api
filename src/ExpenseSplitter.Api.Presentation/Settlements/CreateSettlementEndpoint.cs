using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record CreateSettlementRequest(
    string Name,
    IEnumerable<string> ParticipantNames
);

public class CreateSettlementEndpoint() : Endpoint<CreateSettlementRequest, CreateSettlementCommand, Guid, Guid>(
    Route: "",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Post,
    MapRequest: request => new (request.Name, request.ParticipantNames),
    MapResponse: result => result,
    ErrorStatusCodes: [
        StatusCodes.Status400BadRequest
    ]
);

