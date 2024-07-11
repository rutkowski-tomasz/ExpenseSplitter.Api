using ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class LeaveSettlementEndpoint : EndpointEmptyResponse<Guid, LeaveSettlementCommand>
{
    public override LeaveSettlementCommand MapRequest(Guid source)
    {
        return new(source);
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPost("/{settlementId}/leave", (Guid settlementId, ISender sender)
                => Handle(settlementId, sender))
            .Produces<string>(StatusCodes.Status403Forbidden);
    }
}
