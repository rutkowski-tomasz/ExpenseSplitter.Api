using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Presentation.MediatrEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record JoinSettlementRequest([FromBody] JoinSettlementRequestBody Body);

public record JoinSettlementRequestBody(string InviteCode);

public class CreateExpenseEndpoint() : Endpoint<JoinSettlementRequest, JoinSettlementCommand, Guid, Guid>(
    Endpoints.Settlements.Post("join").ProducesErrorCodes(
        StatusCodes.Status400BadRequest,
        StatusCodes.Status404NotFound
    ),
    request => new (request.Body.InviteCode),
    result => result
);
