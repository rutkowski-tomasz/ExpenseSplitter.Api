using ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

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

public class SettlementReimbrusementEndpoint : Endpoint<Guid, CalculateReimbrusementQuery, CalculateReimbrusementQueryResult, SettlementReimbrusementResponse>
{
    public override CalculateReimbrusementQuery MapRequest(Guid source)
    {
        return new(source);
    }

    public override SettlementReimbrusementResponse MapResponse(CalculateReimbrusementQueryResult source)
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

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapGet("/{settlementId}/reimbrusement", (Guid settlementId, ISender sender)
                => Handle(settlementId, sender))
            .Produces<string>(StatusCodes.Status403Forbidden);
    }
}
