using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Api.Presentation.Extensions;

public static class RouteBuilderExtensions
{
    public static RouteGroupBuilder Settlements(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGroup(nameof(Application.Settlements).ToLower())
            .WithTags(nameof(Application.Settlements))
            .RequireAuthorization();
    }

    public static RouteGroupBuilder Users(this IEndpointRouteBuilder builder)
    {
        return builder
            .MapGroup(nameof(Application.Users).ToLower())
            .WithTags(nameof(Application.Users))
            .RequireRateLimiting(RateLimitingExtensions.IpRateLimiting);
    }

    public static RouteHandlerBuilder Post<TRequest, TCommand, TCommandResult, TResponse>(
        this RouteGroupBuilder builder,
        string pattern
    ) where TCommand : IRequest<Result<TCommandResult>>
    {
        return builder.MapPost(pattern, async (
                TRequest request,
                IHandler<TRequest, TCommand, TCommandResult, TResponse> handler
            ) => await handler.Handle(request))
            .Produces<Ok<TResponse>>();
    }

    public static RouteHandlerBuilder WithErrors(this RouteHandlerBuilder builder, params int[] statusCodes)
    {
        foreach (var statusCode in statusCodes)
        {
            builder = builder.Produces<string>(statusCode);
        }

        return builder;
    }
}
