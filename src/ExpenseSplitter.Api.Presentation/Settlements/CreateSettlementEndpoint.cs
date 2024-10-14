using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record CreateSettlementRequest([FromBody] CreateSettlementRequestBody Body);

public record CreateSettlementRequestBody(
    string Name,
    IEnumerable<string> ParticipantNames
);

public class CreateSettlementEndpoint() : Endpoint<CreateSettlementRequest, CreateSettlementCommand, Guid, Guid>(
    Endpoints.Settlements.Post("").ProducesErrorCodes(
        StatusCodes.Status400BadRequest
    ),
    request => new CreateSettlementCommand(
        request.Body.Name,
        request.Body.ParticipantNames
    ),
    result => result
);

