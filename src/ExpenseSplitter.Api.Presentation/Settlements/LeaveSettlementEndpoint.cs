using ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class LeaveSettlementEndpoint : IEndpoint,
    IMapper<Guid, LeaveSettlementCommand>
{
    public LeaveSettlementCommand Map(Guid source)
    {
        return new(source);
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPost("/{settlementId}/leave", (
                Guid settlementId,
                IHandlerEmptyResponse<
                    Guid,
                    LeaveSettlementCommand
                > handler) => handler.Handle(settlementId)
            )
            .Produces<string>(StatusCodes.Status403Forbidden);
    }
}
