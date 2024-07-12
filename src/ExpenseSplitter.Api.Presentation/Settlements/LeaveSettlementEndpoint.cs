using ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record LeaveSettlementRequest([FromRoute] Guid SettlementId);

public class LeaveSettlementEndpoint() : Endpoint<LeaveSettlementRequest, LeaveSettlementCommand>(
    Endpoints.Settlements.Post("{settlementId}/leave").ProducesErrorCodes(
        StatusCodes.Status403Forbidden
    ),
    MapRequest: request => new (request.SettlementId)
);
