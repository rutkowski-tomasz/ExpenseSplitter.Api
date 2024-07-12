using ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record CalculateReimbrusementRequest([FromRoute] Guid SettlementId);

public record SettlementReimbrusementResponse(
    IEnumerable<SettlementReimbrusementResponseBalance> Balances,
    IEnumerable<SettlementReimbrusementResponseSuggestedReimbrusement> SuggestedReimbrusements
);

public record SettlementReimbrusementResponseBalance(
    Guid ParticipantId,
    decimal Value
);

public record SettlementReimbrusementResponseSuggestedReimbrusement(
    Guid FromParticipantId,
    Guid ToParticipantId,
    decimal Value
);

public class SettlementReimbrusementEndpoint() : Endpoint<CalculateReimbrusementRequest, CalculateReimbrusementQuery, CalculateReimbrusementQueryResult, SettlementReimbrusementResponse>(
    Endpoints.Settlements.Get("{settlementId}/reimbrusement").ProducesErrorCodes(
        StatusCodes.Status403Forbidden
    ),
    request => new (request.SettlementId),
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
