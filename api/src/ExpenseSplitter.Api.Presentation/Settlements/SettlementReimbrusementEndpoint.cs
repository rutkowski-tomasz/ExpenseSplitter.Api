using ExpenseSplitter.Api.Application.Settlements.CalculateReimbursement;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record CalculateReimbursementRequest([FromRoute] Guid SettlementId);

public record SettlementReimbursementResponse(
    IEnumerable<SettlementReimbursementResponseBalance> Balances,
    IEnumerable<SettlementReimbursementResponseSuggestedReimbursement> SuggestedReimbursements
);

public record SettlementReimbursementResponseBalance(
    Guid ParticipantId,
    decimal Value
);

public record SettlementReimbursementResponseSuggestedReimbursement(
    Guid FromParticipantId,
    Guid ToParticipantId,
    decimal Value
);

public class SettlementReimbursementEndpoint() : Endpoint<CalculateReimbursementRequest, CalculateReimbursementQuery, CalculateReimbursementQueryResult, SettlementReimbursementResponse>(
    Endpoints.Settlements.Get("{settlementId}/reimbursement").ProducesErrorCodes([
        StatusCodes.Status403Forbidden
    ]),
    request => new CalculateReimbursementQuery(request.SettlementId),
    result => new SettlementReimbursementResponse(
        result.Balances.Select(balance => new SettlementReimbursementResponseBalance(
            balance.ParticipantId,
            balance.Value
        )),
        result.SuggestedReimbursements.Select(suggestedReimbursement => new SettlementReimbursementResponseSuggestedReimbursement(
            suggestedReimbursement.FromParticipantId,
            suggestedReimbursement.ToParticipantId,
            suggestedReimbursement.Value
        ))
    )
);
