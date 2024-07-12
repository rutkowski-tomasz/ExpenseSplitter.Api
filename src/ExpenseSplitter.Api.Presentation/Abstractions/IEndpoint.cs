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
    AutoEndpoint AutoEndpoint,
    Func<TRequest, TCommand> MapRequest,
    Action<RouteHandlerBuilder>? RouteHandlerCustomization = null
) : IEndpoint
    where TCommand : IRequest<Result>
    where TRequest : notnull
{
    private RouteHandlerBuilder CreateRouteHandlerBuilder(RouteGroupBuilder routeGroupBuilder)
    {
        return AutoEndpoint.Method switch
        {
            EndpointMethod.Get => routeGroupBuilder.MapGet(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Post => routeGroupBuilder.MapPost(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Put => routeGroupBuilder.MapPut(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Delete => routeGroupBuilder.MapDelete(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Patch => routeGroupBuilder.MapPatch(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            _ => throw new NotImplementedException()
        };
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = AutoEndpoint.GroupBuilder(builder);
        var routeHandlerBuilder = CreateRouteHandlerBuilder(routeGroupBuilder);

        if (RouteHandlerCustomization is not null)
        {
            RouteHandlerCustomization(routeHandlerBuilder);
        }

        foreach (var statusCode in AutoEndpoint.ErrorCodes ?? [])
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
    AutoEndpoint AutoEndpoint,
    Func<TRequest, TCommand> MapRequest,
    Func<TCommandResult, TResponse> MapResponse,
    Action<RouteHandlerBuilder>? RouteHandlerCustomization = null
) : IEndpoint
    where TCommand : IRequest<Result<TCommandResult>>
    where TRequest : notnull
{
    private RouteHandlerBuilder CreateRouteHandlerBuilder(RouteGroupBuilder routeGroupBuilder)
    {
        return AutoEndpoint.Method switch
        {
            EndpointMethod.Get => routeGroupBuilder.MapGet(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Post => routeGroupBuilder.MapPost(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Put => routeGroupBuilder.MapPut(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Delete => routeGroupBuilder.MapDelete(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            EndpointMethod.Patch => routeGroupBuilder.MapPatch(AutoEndpoint.Route, ([AsParameters] TRequest request, ISender sender) => Handle(request, sender)),
            _ => throw new NotImplementedException()
        };
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = AutoEndpoint.GroupBuilder(builder);
        var routeHandlerBuilder = CreateRouteHandlerBuilder(routeGroupBuilder);

        if (RouteHandlerCustomization is not null)
        {
            RouteHandlerCustomization(routeHandlerBuilder);
        }

        foreach (var statusCode in AutoEndpoint.ErrorCodes ?? [])
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
    AutoEndpoint AutoEndpoint,
    Func<TCommandResult, TResponse> MapResponse,
    Action<RouteHandlerBuilder>? RouteHandlerCustomization = null
) : IEndpoint
    where TCommand : IRequest<Result<TCommandResult>>, new()
{
    private RouteHandlerBuilder CreateRouteHandlerBuilder(RouteGroupBuilder routeGroupBuilder)
    {
        return AutoEndpoint.Method switch
        {
            EndpointMethod.Get => routeGroupBuilder.MapGet(AutoEndpoint.Route, (ISender sender) => Handle(sender)),
            EndpointMethod.Post => routeGroupBuilder.MapPost(AutoEndpoint.Route, (ISender sender) => Handle(sender)),
            EndpointMethod.Put => routeGroupBuilder.MapPut(AutoEndpoint.Route, (ISender sender) => Handle(sender)),
            EndpointMethod.Delete => routeGroupBuilder.MapDelete(AutoEndpoint.Route, (ISender sender) => Handle(sender)),
            EndpointMethod.Patch => routeGroupBuilder.MapPatch(AutoEndpoint.Route, (ISender sender) => Handle(sender)),
            _ => throw new NotImplementedException()
        };
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = AutoEndpoint.GroupBuilder(builder);
        var routeHandlerBuilder = CreateRouteHandlerBuilder(routeGroupBuilder);

        if (RouteHandlerCustomization is not null)
        {
            RouteHandlerCustomization(routeHandlerBuilder);
        }

        foreach (var statusCode in AutoEndpoint.ErrorCodes ?? [])
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

public enum EndpointMethod
{
    Get,
    Post,
    Put,
    Delete,
    Patch
}

public record AutoEndpointBuilder(Func<IEndpointRouteBuilder, RouteGroupBuilder> GroupBuilder)
{
    public AutoEndpoint Post([StringSyntax("Route")] string route)
        => new(route, EndpointMethod.Post, GroupBuilder);

    public AutoEndpoint Put([StringSyntax("Route")] string route)
        => new(route, EndpointMethod.Put, GroupBuilder);

    public AutoEndpoint Get([StringSyntax("Route")] string route)
        => new(route, EndpointMethod.Get, GroupBuilder);

    public AutoEndpoint Delete([StringSyntax("Route")] string route)
        => new(route, EndpointMethod.Delete, GroupBuilder);

    public AutoEndpoint Patch([StringSyntax("Route")] string route)
        => new(route, EndpointMethod.Patch, GroupBuilder);
}

public record AutoEndpoint(
    string Route,
    EndpointMethod Method,
    Func<IEndpointRouteBuilder, RouteGroupBuilder> GroupBuilder
)
{
    public IEnumerable<int> ErrorCodes { get; private set; }

    public AutoEndpoint ProducesErrorCodes(params int[] errorCodes)
    {
        ErrorCodes = errorCodes;
        return this;
    }
}

public class Endpoints
{
    public static AutoEndpointBuilder Expenses = new(builder => builder
        .MapGroup(nameof(Application.Expenses).ToLower())
        .WithTags(nameof(Application.Expenses))
        .RequireAuthorization()
    );

    public static AutoEndpointBuilder Settlements = new(builder => builder
        .MapGroup(nameof(Application.Settlements).ToLower())
        .WithTags(nameof(Application.Settlements))
        .RequireAuthorization()
    );

    public static AutoEndpointBuilder Users = new(builder => builder
        .MapGroup(nameof(Application.Users).ToLower())
        .WithTags(nameof(Application.Users))
        .RequireRateLimiting(RateLimitingExtensions.IpRateLimiting)
    );
}
