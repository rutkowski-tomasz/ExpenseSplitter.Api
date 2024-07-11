using ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class DeleteSettlementEndpoint : EndpointEmptyResponse<Guid, DeleteSettlementCommand>
{
    public override DeleteSettlementCommand MapRequest(Guid source)
    {
        return new(source);
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapDelete("{settlementId}", (Guid settlementId, ISender sender)
                => Handle(settlementId, sender))
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status412PreconditionFailed)
            .Produces<string>(StatusCodes.Status404NotFound);
    }
}
