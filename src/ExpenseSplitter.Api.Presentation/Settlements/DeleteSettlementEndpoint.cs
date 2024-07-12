using ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class DeleteSettlementEndpoint() : Endpoint<Guid, DeleteSettlementCommand>(
    Endpoints.Settlements.Delete("{settlementId}").ProducesErrorCodes(
        StatusCodes.Status403Forbidden,
        StatusCodes.Status412PreconditionFailed,
        StatusCodes.Status404NotFound
    ),
    request => new (request)
);
