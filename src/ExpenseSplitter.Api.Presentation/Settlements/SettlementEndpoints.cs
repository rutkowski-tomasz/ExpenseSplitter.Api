﻿using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Application.Expenses.GetExpensesForSettlement;
using ExpenseSplitter.Api.Application.Participants.ClaimParticipant;
using ExpenseSplitter.Api.Application.Settlements.CalculateReimbrusement;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;
using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;
using ExpenseSplitter.Api.Application.Settlements.UpdateSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.Settlements.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public static class SettlementEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapSettlementEndpoints(this IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder.MapGroup("api/settlements").RequireAuthorization();

        routeGroupBuilder.MapGet("", GetAllSettlements);
        routeGroupBuilder.MapPost("", CreateSettlement);
        routeGroupBuilder.MapPut("{settlementId}", UpdateSettlement);
        routeGroupBuilder.MapGet("{settlementId}", GetSettlement);
        routeGroupBuilder.MapDelete("{settlementId}", DeleteSettlement);

        routeGroupBuilder.MapGet("{settlementId}/expenses", GetExpensesForSettlement);

        routeGroupBuilder.MapPost("/join", JoinSettlement);
        routeGroupBuilder.MapPost("/{settlementId}/leave", LeaveSettlement);
        routeGroupBuilder.MapPatch("/{settlementId}/participants/{participantId}/claim", ClaimParticipant);
        routeGroupBuilder.MapGet("/{settlementId}/reimbrusement", CalculateReimbrusement);

        return builder;
    }

    public static async Task<Results<Ok<GetAllSettlementsQueryResult>, NotFound>> GetAllSettlements(
        ISender sender,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var query = new GetAllSettlementsQuery(page, pageSize);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.NotFound();
    }

    public static async Task<Results<Ok<GetSettlementQueryResult>, NotFound>> GetSettlement(
        Guid settlementId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new GetSettlementQuery(settlementId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.NotFound();
    }

    public static async Task<Results<Ok, NotFound>> DeleteSettlement(
        Guid settlementId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new DeleteSettlementCommand(settlementId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    public static async Task<Results<Ok<Guid>, BadRequest<Error>>> CreateSettlement(
        CreateSettlementRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] string? requestId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        if (!Guid.TryParse(requestId, out var parsedRequestId))
        {
            return TypedResults.BadRequest(Error.NullValue);
        }

        var command = new CreateSettlementCommand(
            parsedRequestId,
            request.Name,
            request.ParticipantNames
        );

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
    }

    public static async Task<Results<Ok, BadRequest>> UpdateSettlement(
        Guid settlementId,
        UpdateSettlementRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateSettlementCommand(
            settlementId,
            request.Name,
            request.Participants.Select(x => new UpdateSettlementCommandParticipant(
                x.Id,
                x.Nickname
            ))
        );

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.BadRequest();
    }

    public static async Task<Results<Ok<GetExpensesForSettlementQueryResult>, BadRequest<Error>>> GetExpensesForSettlement(
        Guid settlementId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new GetExpensesForSettlementQuery(settlementId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
    }

    public static async Task<Results<Ok<Guid>, BadRequest<Error>>> JoinSettlement(
        JoinSettlementRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new JoinSettlementCommand(request.InviteCode);

        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
    }

    public static async Task<Results<Ok, BadRequest<Error>>> LeaveSettlement(
        Guid settlementId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new LeaveSettlementCommand(settlementId);

        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess ? TypedResults.Ok() : TypedResults.BadRequest(result.Error);
    }

    public static async Task<Results<Ok, BadRequest<Error>>> ClaimParticipant(
        Guid settlementId,
        Guid participantId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new ClaimParticipantCommand(settlementId, participantId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.BadRequest(result.Error);
    }

    public static async Task<Results<Ok<CalculateReimbrusementQueryResult>, BadRequest<Error>>> CalculateReimbrusement(
        Guid settlementId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new CalculateReimbrusementQuery(settlementId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
    }
}
