using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

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

public class GetSettlementEndpoint : Endpoint<Guid, GetSettlementQuery, GetSettlementQueryResult, GetSettlementResponse>
{
    public override GetSettlementQuery MapRequest(Guid source)
    {
        return new GetSettlementQuery(source);
    }

    public override GetSettlementResponse MapResponse(GetSettlementQueryResult source)
    {
        return new GetSettlementResponse(
            source.Id,
            source.Name,
            source.InviteCode,
            source.TotalCost,
            source.YourCost,
            source.Participants.Select(participant => new GetSettlementResponseParticipant(
                participant.Id,
                participant.Nickname
            ))
        );
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapGet("{settlementId}", (Guid settlementId, ISender sender)
                => Handle(settlementId, sender))
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status304NotModified);
    }
}
