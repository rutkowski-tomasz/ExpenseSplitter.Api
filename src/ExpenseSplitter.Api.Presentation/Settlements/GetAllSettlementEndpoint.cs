using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record GetAllSettlementsRequest(int Page, int PageSize);

public record GetAllSettlementsResponse(
    IEnumerable<GetAllSettlementsResponseSettlement> Settlements
);

public record GetAllSettlementsResponseSettlement(
    Guid Id,
    string Name
);

public class GetAllSettlementsEndpoint : IEndpoint,
    IMapper<GetAllSettlementsRequest, GetAllSettlementsQuery>,
    IMapper<GetAllSettlementsQueryResult, GetAllSettlementsResponse>
{
    public GetAllSettlementsQuery Map(GetAllSettlementsRequest source)
    {
        return new GetAllSettlementsQuery(source.Page, source.PageSize);
    }

    public GetAllSettlementsResponse Map(GetAllSettlementsQueryResult source)
    {
        return new GetAllSettlementsResponse(
            source.Settlements.Select(settlement => new GetAllSettlementsResponseSettlement(
                settlement.Id,
                settlement.Name
            ))
        );
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapGet("", (
                [AsParameters] GetAllSettlementsRequest request,
                IHandler<
                    GetAllSettlementsRequest,
                    GetAllSettlementsQuery,
                    GetAllSettlementsQueryResult,
                    GetAllSettlementsResponse
                > handler) => handler.Handle(request)
            )
            .RequireRateLimiting(RateLimitingExtensions.UserRateLimiting);
    }
}
