using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Abstractions;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder builder);
}

public abstract class Endpoint<TRequest, TCommand>(
    [StringSyntax("Route")] 
    string Route,
    EndpointGroup Group,
    EndpointMethod Method,
    Func<TRequest, TCommand> MapRequest,
    IEnumerable<int>? ErrorStatusCodes
) : IEndpoint
    where TCommand : IRequest<Result>
    where TRequest : notnull
{
    private RouteHandlerBuilder CreateRouteHandlerBuilder(RouteGroupBuilder routeGroupBuilder)
    {
        return Method switch
        {
            EndpointMethod.Get => routeGroupBuilder.MapGet(Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Post => routeGroupBuilder.MapPost(Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Put => routeGroupBuilder.MapPut(Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Delete => routeGroupBuilder.MapDelete(Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            _ => throw new NotImplementedException()
        };
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = Group.Map(builder);
        var routeHandlerBuilder = CreateRouteHandlerBuilder(routeGroupBuilder);

        foreach (var statusCode in ErrorStatusCodes ?? [])
        {
            routeHandlerBuilder.Produces<string>(statusCode);
        }
    }

    protected async Task<IResult> Handle(TRequest request, ISender sender)
    {
        var command = MapRequest(request);
        var result = await sender.Send(command);
        return result.ToHttpResult();
    }
}

public abstract class Endpoint<TRequest, TCommand, TCommandResult, TResponse>(
    [StringSyntax("Route")] 
    string Route,
    EndpointGroup Group,
    EndpointMethod Method,
    Func<TRequest, TCommand> MapRequest,
    Func<TCommandResult, TResponse> MapResponse,
    IEnumerable<int>? ErrorStatusCodes
) : IEndpoint
    where TCommand : IRequest<Result<TCommandResult>>
    where TRequest : notnull
{
    private RouteHandlerBuilder CreateRouteHandlerBuilder(RouteGroupBuilder routeGroupBuilder)
    {
        return Method switch
        {
            EndpointMethod.Get => routeGroupBuilder.MapGet(Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Post => routeGroupBuilder.MapPost(Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Put => routeGroupBuilder.MapPut(Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Delete => routeGroupBuilder.MapDelete(Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            _ => throw new NotImplementedException()
        };
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = Group.Map(builder);
        var routeHandlerBuilder = CreateRouteHandlerBuilder(routeGroupBuilder);

        foreach (var statusCode in ErrorStatusCodes ?? [])
        {
            routeHandlerBuilder.Produces<string>(statusCode);
        }
    }

    protected async Task<IResult> Handle(TRequest request, ISender sender)
    {
        var command = MapRequest(request);
        var result = await sender.Send(command);
        return result.ToHttpResult(MapResponse);
    }
}

public abstract class Endpoint<TCommand, TCommandResult, TResponse>(
    [StringSyntax("Route")] 
    string Route,
    EndpointGroup Group,
    EndpointMethod Method,
    Func<TCommandResult, TResponse> MapResponse,
    IEnumerable<int>? ErrorStatusCodes
) : IEndpoint
    where TCommand : IRequest<Result<TCommandResult>>, new()
{
    private RouteHandlerBuilder CreateRouteHandlerBuilder(RouteGroupBuilder routeGroupBuilder)
    {
        return Method switch
        {
            EndpointMethod.Get => routeGroupBuilder.MapGet(Route, (ISender sender) => Handle(sender)),
            EndpointMethod.Post => routeGroupBuilder.MapPost(Route, (ISender sender) => Handle(sender)),
            EndpointMethod.Put => routeGroupBuilder.MapPut(Route, (ISender sender) => Handle(sender)),
            EndpointMethod.Delete => routeGroupBuilder.MapDelete(Route, (ISender sender) => Handle(sender)),
            _ => throw new NotImplementedException()
        };
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = Group.Map(builder);
        var routeHandlerBuilder = CreateRouteHandlerBuilder(routeGroupBuilder);

        foreach (var statusCode in ErrorStatusCodes ?? [])
        {
            routeHandlerBuilder.Produces<string>(statusCode);
        }
    }

    protected async Task<IResult> Handle(ISender sender)
    {
        var command = new TCommand();
        var result = await sender.Send(command);
        return result.ToHttpResult(MapResponse);
    }
}

public record EndpointDefinition<TRequest, TCommand>(
    [StringSyntax("Route")] 
    string Route,
    EndpointGroup Group,
    EndpointMethod Method,
    Func<TRequest, TCommand> MapRequest,
    IEnumerable<int>? ErrorStatusCodes
);

public enum EndpointMethod
{
    Get,
    Post,
    Put,
    Delete
}

public class EndpointGroup(Func<IEndpointRouteBuilder, RouteGroupBuilder> Map)
{
    public static EndpointGroup Expenses = new(builder => builder
        .MapGroup(nameof(Application.Expenses).ToLower())
        .WithTags(nameof(Application.Expenses))
        .RequireAuthorization()
    );

    public static EndpointGroup Settlements = new(builder => builder
        .MapGroup(nameof(Application.Settlements).ToLower())
        .WithTags(nameof(Application.Settlements))
        .RequireAuthorization()
    );

    public static EndpointGroup Users = new(builder => builder
        .MapGroup(nameof(Application.Users).ToLower())
        .WithTags(nameof(Application.Users))
        .RequireRateLimiting(RateLimitingExtensions.IpRateLimiting)
    );

    public Func<IEndpointRouteBuilder, RouteGroupBuilder> Map { get; } = Map;
}
