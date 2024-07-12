using ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;
using ExpenseSplitter.Api.Presentation.Abstractions;

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

public class SettlementReimbrusementEndpoint() : Endpoint<Guid, CalculateReimbrusementQuery, CalculateReimbrusementQueryResult, SettlementReimbrusementResponse>(
    Endpoints.Settlements.Get("{settlementId}/reimbrusement").ProducesErrorCodes(
        StatusCodes.Status403Forbidden
    ),
    request => new (request),
    result => new(
        result.Balances.Select(balance => new SettlementReimbrusementResponseBalance(
            balance.ParticipantId,
            balance.Value
        )),
        result.SuggestedReimbrusements.Select(suggestedReimbrusement => new SettlementReimbrusementResponseSuggestedReimbrusement(
            suggestedReimbrusement.FromParticipantId,
            suggestedReimbrusement.ToParticipantId,
            suggestedReimbrusement.Value
        ))
    )
);
