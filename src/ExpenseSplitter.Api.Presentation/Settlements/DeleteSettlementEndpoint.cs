using ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record DeleteSettlementRequest([FromRoute] Guid SettlementId);

public class DeleteSettlementEndpoint() : Endpoint<DeleteSettlementRequest, DeleteSettlementCommand>(
    Endpoints.Settlements.Delete("{settlementId}").ProducesErrorCodes(
        StatusCodes.Status403Forbidden,
        StatusCodes.Status412PreconditionFailed,
        StatusCodes.Status404NotFound
    ),
    request => new (request.SettlementId)
);
