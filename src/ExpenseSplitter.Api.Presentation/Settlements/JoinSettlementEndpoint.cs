using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record JoinSettlementRequest(string InviteCode);

public class JoinSettlementEndpoint : IEndpoint,
    IMapper<JoinSettlementRequest, JoinSettlementCommand>
{
    public JoinSettlementCommand Map(JoinSettlementRequest source)
    {
        return new(source.InviteCode);
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPost("join", (
                JoinSettlementRequest request,
                IHandler<
                    JoinSettlementRequest,
                    JoinSettlementCommand,
                    Guid,
                    Guid
                > handler) => handler.Handle(request)
            )
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
