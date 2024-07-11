using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record GetSettlementResponse(
    Guid Id,
    string Name,
    string InviteCode,
    decimal TotalCost,
    decimal? YourCost,
    IEnumerable<GetSettlementResponseParticipant> Participants
);

public sealed record GetSettlementResponseParticipant(
    Guid Id,
    string Nickname
);

public class GetSettlementEndpoint() : Endpoint<Guid, GetSettlementQuery, GetSettlementQueryResult, GetSettlementResponse>(
    Route: "{settlementId}",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Get,
    MapRequest: request => new (request),
    MapResponse: result => new (
        result.Id,
        result.Name,
        result.InviteCode,
        result.TotalCost,
        result.YourCost,
        result.Participants.Select(participant => new GetSettlementResponseParticipant(
            participant.Id,
            participant.Nickname
        ))
    ),
    ErrorStatusCodes: [
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound,
        StatusCodes.Status304NotModified
    ]
);
