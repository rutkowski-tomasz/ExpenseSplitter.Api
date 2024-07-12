using ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class LeaveSettlementEndpoint() : Endpoint<Guid, LeaveSettlementCommand>(
    Endpoints.Settlements.Post("{settlementId}/leave").ProducesErrorCodes(
        StatusCodes.Status403Forbidden
    ),
    MapRequest: request => new (request)
);
