using ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record UpdateSettlementRequest(
    [FromRoute] Guid SettlementId,
    [FromBody] UpdateSettlementRequestBody Body
);

public sealed record UpdateSettlementRequestBody(
    string Name,
    IEnumerable<UpdateSettlementRequestParticipant> Participants
);

public sealed record UpdateSettlementRequestParticipant(
    Guid? Id,
    string Nickname
);

public class UpdateSettlementEndpoint() : Endpoint<UpdateSettlementRequest, UpdateSettlementCommand>(
    Route: "{settlementId}",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Put,
    MapRequest: request => new(
        request.SettlementId,
        request.Body.Name,
        request.Body.Participants.Select(x => new UpdateSettlementCommandParticipant(
            x.Id,
            x.Nickname
        ))
    ),
    ErrorStatusCodes: [
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ]
);
