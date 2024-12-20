using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public record EndpointDefinition
{
    public Func<IEndpointRouteBuilder, IEndpointRouteBuilder> GroupBuilder { get; private init; }
    public Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> Map { get; init; }
    public string Route { get; init; }
    public IEnumerable<int> ErrorCodes { get; private set; } = [];

    public static readonly Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapGet =
        (builder, route, handler) => builder.MapGet(route, handler);
    public static readonly Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapPost =
        (builder, route, handler) => builder.MapPost(route, handler);
    public static readonly Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapPut =
        (builder, route, handler) => builder.MapPut(route, handler);
    public static readonly Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapPatch =
        (builder, route, handler) => builder.MapPatch(route, handler);
    public static readonly Func<IEndpointRouteBuilder, string, Delegate, RouteHandlerBuilder> MapDelete =
        (builder, route, handler) => builder.MapDelete(route, handler);

    private EndpointDefinition()
    {
    }

    public static EndpointDefinition CreateGroup(Func<IEndpointRouteBuilder, IEndpointRouteBuilder> groupBuilder)
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

    public EndpointDefinition ProducesErrorCodes(ReadOnlySpan<int> errorCodes)
    {
        ErrorCodes = errorCodes.ToArray();
        return this;
    }
}
