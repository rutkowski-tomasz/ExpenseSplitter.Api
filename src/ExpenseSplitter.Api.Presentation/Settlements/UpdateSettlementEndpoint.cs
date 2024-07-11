using ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;
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

public class UpdateSettlementEndpoint : EndpointEmptyResponse<UpdateSettlementRequest, UpdateSettlementCommand>
{
    public override UpdateSettlementCommand MapRequest(UpdateSettlementRequest source)
    {
        return new(
            source.SettlementId,
            source.Body.Name,
            source.Body.Participants.Select(x => new UpdateSettlementCommandParticipant(
                x.Id,
                x.Nickname
            ))
        );
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPut("{settlementId}", ([AsParameters] UpdateSettlementRequest request, ISender sender)
                => Handle(request, sender))
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
