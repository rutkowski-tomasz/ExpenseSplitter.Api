using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public record EndpointDefinition
{
    public Func<IEndpointRouteBuilder, RouteGroupBuilder> GroupBuilder { get; set; }
    public Func<RouteGroupBuilder, string, Delegate, RouteHandlerBuilder> Map { get; set; }
    public string Route { get; set; }
    public IEnumerable<int> ErrorCodes { get; set; } = [];

    public readonly static Func<RouteGroupBuilder, string, Delegate, RouteHandlerBuilder> MapGet =
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapGet(route, handler);
    public readonly static Func<RouteGroupBuilder, string, Delegate, RouteHandlerBuilder> MapPost =
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapPost(route, handler);
    public readonly static Func<RouteGroupBuilder, string, Delegate, RouteHandlerBuilder> MapPut =
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapPut(route, handler);
    public readonly static Func<RouteGroupBuilder, string, Delegate, RouteHandlerBuilder> MapPatch =
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapPatch(route, handler);
    public readonly static Func<RouteGroupBuilder, string, Delegate, RouteHandlerBuilder> MapDelete =
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapDelete(route, handler);

    private EndpointDefinition()
    {
    }

    public static EndpointDefinition CreateGroup(Func<IEndpointRouteBuilder, RouteGroupBuilder> groupBuilder)
    {
        return new EndpointDefinition { GroupBuilder = groupBuilder };
    }

    public static EndpointDefinition Post([StringSyntax("Route")] string route)
    {
        return new EndpointDefinition { Route = route, Map = MapPost };
    }

    public static EndpointDefinition Get([StringSyntax("Route")] string route)
    {
        return new EndpointDefinition { Route = route, Map = MapGet };
    }

    public static EndpointDefinition Put([StringSyntax("Route")] string route)
    {
        return new EndpointDefinition { Route = route, Map = MapPut };
    }

    public static EndpointDefinition Patch([StringSyntax("Route")] string route)
    {
        return new EndpointDefinition { Route = route, Map = MapPatch };
    }

    public static EndpointDefinition Delete([StringSyntax("Route")] string route)
    {
        return new EndpointDefinition { Route = route, Map = MapDelete };
    }

    public EndpointDefinition ProducesErrorCodes(params int[] errorCodes)
    {
        ErrorCodes = errorCodes;
        return this;
    }
}
