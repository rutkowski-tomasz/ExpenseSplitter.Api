using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public record GetAllSettlementsRequest(int Page, int PageSize);

public record GetAllSettlementsResponse(
    IEnumerable<GetAllSettlementsResponseSettlement> Settlements
);

public record GetAllSettlementsResponseSettlement(
    Guid Id,
    string Name
);

public class GetAllSettlementsEndpoint : Endpoint<GetAllSettlementsRequest, GetAllSettlementsQuery, GetAllSettlementsQueryResult, GetAllSettlementsResponse>
{
    public override GetAllSettlementsQuery MapRequest(GetAllSettlementsRequest source)
    {
        return new GetAllSettlementsQuery(source.Page, source.PageSize);
    }

    public override GetAllSettlementsResponse MapResponse(GetAllSettlementsQueryResult source)
    {
        return new GetAllSettlementsResponse(
            source.Settlements.Select(settlement => new GetAllSettlementsResponseSettlement(
                settlement.Id,
                settlement.Name
            ))
        );
    }

    public override void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder
            .Settlements()
            .MapGet("", ([AsParameters] GetAllSettlementsRequest request, ISender sender)
                => Handle(request, sender))
            .RequireRateLimiting(RateLimitingExtensions.UserRateLimiting);
    }
}
