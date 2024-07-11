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
    Route: "{settlementId}/reimbrusement",
    Group: EndpointGroup.Settlements,
    Method: EndpointMethod.Get,
    MapRequest: request => new (request),
    MapResponse: result => new(
        result.Balances.Select(balance => new SettlementReimbrusementResponseBalance(
            balance.ParticipantId,
            balance.Value
        )),
        result.SuggestedReimbrusements.Select(suggestedReimbrusement => new SettlementReimbrusementResponseSuggestedReimbrusement(
            suggestedReimbrusement.FromParticipantId,
            suggestedReimbrusement.ToParticipantId,
            suggestedReimbrusement.Value
        ))
    ),
    ErrorStatusCodes: [
        StatusCodes.Status403Forbidden,
    ]
);
