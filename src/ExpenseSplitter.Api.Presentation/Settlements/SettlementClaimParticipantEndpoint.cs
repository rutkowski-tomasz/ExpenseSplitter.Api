using ExpenseSplitter.Api.Application.Participants.ClaimParticipant;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record SettlementlementClaimParticipantRequest(
    [FromRoute] Guid SettlementId,
    [FromRoute] Guid ParticipantId
);

public class SettlementClaimParticipantEndpoint : EndpointEmptyResponse<SettlementlementClaimParticipantRequest, ClaimParticipantCommand>
{
    public override ClaimParticipantCommand MapRequest(SettlementlementClaimParticipantRequest source)
    {
        return new(source.SettlementId, source.ParticipantId);
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPatch("/{settlementId}/participants/{participantId}/claim", (
                [AsParameters] SettlementlementClaimParticipantRequest request, ISender sender)
                => Handle(request, sender))
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
