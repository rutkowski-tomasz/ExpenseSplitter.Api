using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public sealed record CreateSettlementRequest(
    string Name,
    IEnumerable<string> ParticipantNames
);

public class CreateSettlementRequestMapper : IMapper<CreateSettlementRequest, CreateSettlementCommand>
{
    public CreateSettlementCommand Map(CreateSettlementRequest source) =>
        new (
            source.Name,
            source.ParticipantNames
        );
}

public class CreateSettlementResponseMapper : IMapper<Guid, Guid>
{
    public Guid Map(Guid source) => source;
}

public class CreateSettlementEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .Post<CreateSettlementRequest, CreateSettlementCommand, Guid, Guid>("test")
            .WithErrors(StatusCodes.Status400BadRequest);
    }
}
