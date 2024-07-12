using ExpenseSplitter.Api.Application.Participants.ClaimParticipant;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record SettlementlementClaimParticipantRequest(
    [FromRoute] Guid SettlementId,
    [FromRoute] Guid ParticipantId
);

public class SettlementClaimParticipantEndpoint() : Endpoint<SettlementlementClaimParticipantRequest, ClaimParticipantCommand>(
    Endpoints.Settlements.Patch("{settlementId}/participants/{participantId}/claim").ProducesErrorCodes(
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound
    ),
    request => new(request.SettlementId, request.ParticipantId)
);
