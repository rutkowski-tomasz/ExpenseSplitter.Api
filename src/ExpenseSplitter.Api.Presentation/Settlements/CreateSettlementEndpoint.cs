using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record CreateSettlementRequest(
    string Name,
    IEnumerable<string> ParticipantNames
);

public class CreateSettlementEndpoint : Endpoint<CreateSettlementRequest, CreateSettlementCommand, Guid, Guid>
{
    public override CreateSettlementCommand MapRequest(CreateSettlementRequest request)
    {
        return new CreateSettlementCommand(
            request.Name,
            request.ParticipantNames
        );
    }

    public override Guid MapResponse(Guid result) => result;

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapPost("", (CreateSettlementRequest request, ISender sender) => 
                Handle(request, sender))
            .Produces<string>(StatusCodes.Status400BadRequest);
    }
}
