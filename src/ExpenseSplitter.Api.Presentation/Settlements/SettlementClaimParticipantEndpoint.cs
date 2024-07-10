using ExpenseSplitter.Api.Application.Participants.ClaimParticipant;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record SettlementlementClaimParticipantRequest(
    [FromRoute] Guid SettlementId,
    [FromRoute] Guid ParticipantId
);

public class SettlementClaimParticipantEndpoint : IEndpoint,
    IMapper<SettlementlementClaimParticipantRequest, ClaimParticipantCommand>
{
    public ClaimParticipantCommand Map(SettlementlementClaimParticipantRequest source)
    {
        return new(source.SettlementId, source.ParticipantId);
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPatch("/{settlementId}/participants/{participantId}/claim", (
                [AsParameters] SettlementlementClaimParticipantRequest request,
                IHandlerEmptyResponse<
                    SettlementlementClaimParticipantRequest,
                    ClaimParticipantCommand
                > handler) => handler.Handle(request)
            )
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
