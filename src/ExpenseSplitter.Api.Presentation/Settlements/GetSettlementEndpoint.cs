using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record GetSettlementRequest([FromRoute] Guid SettlementId);

public record GetSettlementResponse(
    Guid Id,
    string Name,
    string InviteCode,
    decimal TotalCost,
    decimal? YourCost,
    IEnumerable<GetSettlementResponseParticipant> Participants
);

public record GetSettlementResponseParticipant(
    Guid Id,
    string Nickname
);

public class GetSettlementEndpoint() : Endpoint<GetSettlementRequest, GetSettlementQuery, GetSettlementQueryResult, GetSettlementResponse>(
    Endpoints.Settlements.Get("{settlementId}").ProducesErrorCodes(
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound,
        StatusCodes.Status304NotModified
    ),
    request => new GetSettlementQuery(request.SettlementId),
    result => new GetSettlementResponse(
        result.Id,
        result.Name,
        result.InviteCode,
        result.TotalCost,
        result.YourCost,
        result.Participants.Select(participant => new GetSettlementResponseParticipant(
            participant.Id,
            participant.Nickname
        ))
    )
);
