using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public record EndpointDefinition
{
    public Func<IEndpointRouteBuilder, RouteGroupBuilder> GroupBuilder { get; set; }
    public Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> Map { get; set; }
    public string Route { get; set; }
    public IEnumerable<int> ErrorCodes { get; set; } = [];

    public readonly static Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapGet =
        (IEndpointRouteBuilder builder, string route, Delegate handler) => builder.MapGet(route, handler);
    public readonly static Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapPost =
        (IEndpointRouteBuilder builder, string route, Delegate handler) => builder.MapPost(route, handler);
    public readonly static Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapPut =
        (IEndpointRouteBuilder builder, string route, Delegate handler) => builder.MapPut(route, handler);
    public readonly static Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapPatch =
        (IEndpointRouteBuilder builder, string route, Delegate handler) => builder.MapPatch(route, handler);
    public readonly static Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapDelete =
        (IEndpointRouteBuilder builder, string route, Delegate handler) => builder.MapDelete(route, handler);

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
