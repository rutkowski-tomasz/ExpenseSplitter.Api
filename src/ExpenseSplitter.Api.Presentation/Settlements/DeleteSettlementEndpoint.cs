using ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class DeleteSettlementEndpoint : IEndpoint,
    IMapper<Guid, DeleteSettlementCommand>
{
    public DeleteSettlementCommand Map(Guid source)
    {
        return new(source);
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapDelete("{settlementId}", (
                Guid settlementId,
                IHandlerEmptyResponse<
                    Guid,
                    DeleteSettlementCommand
                > handler) => handler.Handle(settlementId)
            )
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status412PreconditionFailed)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
