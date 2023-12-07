using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Application.Participants.ClaimParticipant;
using ExpenseSplitter.Api.Application.Participants.GetParticipantsBySettlementId;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Api.Presentation.Settlements;

public static class ParticipantsEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapParticipantEndpoints(this IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder.MapGroup("api/participants").RequireAuthorization();

        routeGroupBuilder.MapGet("{settlementId}", GetSettlementParticipants);
        routeGroupBuilder.MapPost("{settlementId}/{participantId}", ClaimParticipant);

        return builder;
    }

    public static async Task<Results<Ok<GetParticipantsBySettlementIdQueryResult>, BadRequest<Error>>> GetSettlementParticipants(
        Guid settlementId,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var query = new GetParticipantsBySettlementIdQuery(settlementId);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
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

    

}
