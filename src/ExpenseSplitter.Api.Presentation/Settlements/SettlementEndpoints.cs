using System.Diagnostics.CodeAnalysis;
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
using ExpenseSplitter.Api.Presentation.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using ExpenseSplitter.Api.Presentation.Settlements.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public class SettlementEndpoints : IEndpoint
{
    public record GetAllSettlementsRequest(int Page, int PageSize);

    [ExcludeFromCodeCoverage]
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder
            .MapGroup("settlements")
            .WithTags(nameof(SettlementEndpoints))
            .RequireAuthorization();

        // routeGroupBuilder.MapGet("", async
        //     ([AsParameters] GetAllSettlementsRequest request, ISender sender, CancellationToken cancellationToken) => {
        //         var query = new GetAllSettlementsQuery(request.Page, request.PageSize);
        //         var result = await sender.Send(query, cancellationToken);
        //         return result.ToHttpResult();
        //     })
        //     .Produces<Ok<GetAllSettlementsQueryResult>>()
        //     .RequireRateLimiting(RateLimitingExtensions.UserRateLimiting);

        // builder.Settlements().MapPost("test", async (CreateSettlementRequest request, CancellationToken ct,
        //     IHandler<CreateSettlementRequest, CreateSettlementCommand, Guid, Guid> handler) =>
        //         await handler.Handle(request, ct))
        //     .WithErrors<Guid>(StatusCodes.Status400BadRequest);

        routeGroupBuilder.MapPost("", async
            (CreateSettlementRequest request, ISender sender, CancellationToken cancellationToken) => {
                var command = new CreateSettlementCommand(
                    request.Name,
                    request.ParticipantNames
                );

                var result = await sender.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok<Guid>>()
            .Produces<string>(StatusCodes.Status400BadRequest);

        routeGroupBuilder.MapPut("{settlementId}", async
            (Guid settlementId, UpdateSettlementRequest request, ISender sender, CancellationToken cancellationToken) => {

                var command = new UpdateSettlementCommand(
                    settlementId,
                    request.Name,
                    request.Participants.Select(x => new UpdateSettlementCommandParticipant(
                        x.Id,
                        x.Nickname
                    ))
                );

                var result = await sender.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok>()
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("{settlementId}", async
            (Guid settlementId, ISender sender, CancellationToken cancellationToken) => {
                var query = new GetSettlementQuery(settlementId);
                var result = await sender.Send(query, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok<GetSettlementQueryResult>>()
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status304NotModified);

        routeGroupBuilder.MapDelete("{settlementId}", async
            (Guid settlementId, ISender sender, CancellationToken cancellationToken) => {
                var query = new DeleteSettlementCommand(settlementId);
                var result = await sender.Send(query, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok>()
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status412PreconditionFailed)
            .Produces<string>(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("{settlementId}/expenses", async
            (Guid settlementId, ISender sender, CancellationToken cancellationToken) => {
                var query = new GetExpensesForSettlementQuery(settlementId);
                var result = await sender.Send(query, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok<GetExpensesForSettlementQueryResult>>();
        
        routeGroupBuilder.MapPost("/join", async
            (JoinSettlementRequest request, ISender sender, CancellationToken cancellationToken) => {
                var command = new JoinSettlementCommand(request.InviteCode);
                var result = await sender.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok<Guid>>()
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapPost("/{settlementId}/leave", async
            (Guid settlementId, ISender sender, CancellationToken cancellationToken) => {
                var command = new LeaveSettlementCommand(settlementId);
                var result = await sender.Send(command, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok>()
            .Produces<string>(StatusCodes.Status403Forbidden);

        routeGroupBuilder.MapPatch("/{settlementId}/participants/{participantId}/claim", async
            (Guid settlementId, Guid participantId, ISender sender, CancellationToken cancellationToken) => {
                var query = new ClaimParticipantCommand(settlementId, participantId);
                var result = await sender.Send(query, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok>()
            .Produces<string>(StatusCodes.Status403Forbidden)
            .Produces<string>(StatusCodes.Status404NotFound);

        routeGroupBuilder.MapGet("/{settlementId}/reimbrusement", async
            (Guid settlementId, ISender sender, CancellationToken cancellationToken) => {
                var query = new CalculateReimbrusementQuery(settlementId);
                var result = await sender.Send(query, cancellationToken);
                return result.ToHttpResult();
            })
            .Produces<Ok<CalculateReimbrusementQueryResult>>()
            .Produces<string>(StatusCodes.Status403Forbidden);
    }
}
