using ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
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

public class UpdateSettlementEndpoint : IEndpoint,
    IMapper<UpdateSettlementRequest, UpdateSettlementCommand>
{
    public UpdateSettlementCommand Map(UpdateSettlementRequest source)
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

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPut("{settlementId}", (
                [AsParameters] UpdateSettlementRequest request,
                IHandlerEmptyResponse<
                    UpdateSettlementRequest,
                    UpdateSettlementCommand
                > handler) => handler.Handle(request)
            )
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
