using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.Settlements.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public static class SettlementEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapSettlementEndpoints(this IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder.MapGroup("api/settlements").RequireAuthorization();

        routeGroupBuilder.MapGet("", GetAllSettlements);
        routeGroupBuilder.MapGet("{id}", GetSettlement);
        routeGroupBuilder.MapDelete("{id}", DeleteSettlement);
        routeGroupBuilder.MapPost("", CreateSettlement);
        routeGroupBuilder.MapPost("/join", JoinSettlement);

        return builder;
    }

    public static async Task<Results<Ok<GetAllSettlementsQueryResponse>, NotFound>> GetAllSettlements(
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

    public static async Task<Results<Ok, NotFound>> DeleteSettlement(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new DeleteSettlementCommand(id);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    public static async Task<Results<Ok<Guid>, BadRequest>> CreateSettlement(
        CreateSettlementRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateSettlementCommand(request.Name, request.ParticipantNames);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest();
    }

    public static async Task<Results<Ok<Guid>, BadRequest<Error>>> JoinSettlement(
        JoinSettlementRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new JoinSettlementCommand(request.InviteCode, request.Nickname);

        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
    }
}