using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

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

public class GetSettlementEndpoint : IEndpoint,
    IMapper<Guid, GetSettlementQuery>,
    IMapper<GetSettlementQueryResult, GetSettlementResponse>
{
    public GetSettlementQuery Map(Guid source)
    {
        return new GetSettlementQuery(source);
    }

    public GetSettlementResponse Map(GetSettlementQueryResult source)
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

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapGet("{settlementId}", (
                Guid settlementId,
                IHandler<
                    Guid,
                    GetSettlementQuery,
                    GetSettlementQueryResult,
                    GetSettlementResponse
                > handler) => handler.Handle(settlementId)
            )
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status304NotModified);
    }
}
