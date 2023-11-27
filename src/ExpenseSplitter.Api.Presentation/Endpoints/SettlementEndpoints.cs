using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Api.Presentation.Endpoints;

public static class SettlementEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapSettlementEndpoints(this IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder.MapGroup("api/settlements").RequireAuthorization();

        routeGroupBuilder.MapGet("", GetAllSettlements);
        routeGroupBuilder.MapGet("{id}", GetSettlement);
        routeGroupBuilder.MapPost("", CreateSettlement);

        return builder;
    }

    public static async Task<Results<Ok<IEnumerable<GetSettlementResponse>>, NotFound>> GetAllSettlements(
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new GetAllSettlementsQuery();

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.NotFound();
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

    public static async Task<Results<Ok<Guid>, BadRequest>> CreateSettlement(
        CreateSettlementRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateSettlementCommand(request.Name);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest();
    }
}