using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record JoinSettlementRequest(string InviteCode);

public class JoinSettlementEndpoint : EndpointEmptyResponse<JoinSettlementRequest, JoinSettlementCommand>
{
    public override JoinSettlementCommand MapRequest(JoinSettlementRequest source)
    {
        return new(source.InviteCode);
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPost("join", (JoinSettlementRequest request, ISender sender)
                => Handle(request, sender))
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
