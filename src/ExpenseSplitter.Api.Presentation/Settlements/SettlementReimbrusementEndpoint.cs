using ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record SettlementReimbrusementResponse(
    IEnumerable<SettlementReimbrusementResponseBalance> Balances,
    IEnumerable<SettlementReimbrusementResponseSuggestedReimbrusement> SuggestedReimbrusements
);

public sealed record SettlementReimbrusementResponseBalance(
    Guid ParticipantId,
    decimal Value
);

public sealed record SettlementReimbrusementResponseSuggestedReimbrusement(
    Guid FromParticipantId,
    Guid ToParticipantId,
    decimal Value
);

public class SettlementReimbrusementEndpoint : IEndpoint,
    IMapper<Guid, CalculateReimbrusementQuery>,
    IMapper<CalculateReimbrusementQueryResult, SettlementReimbrusementResponse>
{
    public CalculateReimbrusementQuery Map(Guid source)
    {
        return new(source);
    }

    public SettlementReimbrusementResponse Map(CalculateReimbrusementQueryResult source)
    {
        return new(
            source.Balances.Select(balance => new SettlementReimbrusementResponseBalance(
                balance.ParticipantId,
                balance.Value
            )),
            source.SuggestedReimbrusements.Select(suggestedReimbrusement => new SettlementReimbrusementResponseSuggestedReimbrusement(
                suggestedReimbrusement.FromParticipantId,
                suggestedReimbrusement.ToParticipantId,
                suggestedReimbrusement.Value
            ))
        );
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapGet("/{settlementId}/reimbrusement", (
                Guid settlementId,
                IHandler<
                    Guid,
                    CalculateReimbrusementQuery,
                    CalculateReimbrusementQueryResult,
                    SettlementReimbrusementResponse
                > handler) => handler.Handle(settlementId)
            )
            .Produces<string>(StatusCodes.Status403Forbidden);
    }
}
