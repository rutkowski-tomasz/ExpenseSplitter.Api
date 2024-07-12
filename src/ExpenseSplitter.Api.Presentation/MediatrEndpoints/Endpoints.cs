using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public partial record Endpoints
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

    private Endpoints()
    {
    }

    public static Endpoints CreateGroup(Func<IEndpointRouteBuilder, RouteGroupBuilder> groupBuilder)
    {
        return new Endpoints { GroupBuilder = groupBuilder };
    }

    public static Endpoints Post([StringSyntax("Route")] string route)
    {
        return new Endpoints { Route = route, Map = MapPost };
    }

    public static Endpoints Get([StringSyntax("Route")] string route)
    {
        return new Endpoints { Route = route, Map = MapGet };
    }

    public static Endpoints Put([StringSyntax("Route")] string route)
    {
        return new Endpoints { Route = route, Map = MapPut };
    }

    public static Endpoints Patch([StringSyntax("Route")] string route)
    {
        return new Endpoints { Route = route, Map = MapPatch };
    }

    public static Endpoints Delete([StringSyntax("Route")] string route)
    {
        return new Endpoints { Route = route, Map = MapDelete };
    }

    public Endpoints ProducesErrorCodes(params int[] errorCodes)
    {
        ErrorCodes = errorCodes;
        return this;
    }
}
