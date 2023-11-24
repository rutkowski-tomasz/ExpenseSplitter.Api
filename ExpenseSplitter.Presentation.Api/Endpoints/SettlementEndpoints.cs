using ExpenseSplitter.Application.Settlements.GetSettlement;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Presentation.Api.Endpoints;

public static class SettlementEndpoints
{
    public static IEndpointRouteBuilder MapSettlementEndpoints(this IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder.MapGroup("api/settlements");

        routeGroupBuilder.MapGet("{id}", GetSettlement);

        return builder;
    }

    public static async Task<Results<Ok<GetSettlementResponse>, NotFound>> GetSettlement(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new GetSettlementQuery(id);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.NotFound();
    }
}