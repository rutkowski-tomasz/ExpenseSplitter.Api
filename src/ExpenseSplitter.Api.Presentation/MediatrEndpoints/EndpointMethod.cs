namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public record EndpointMethod(Func<RouteGroupBuilder, string, Delegate, RouteHandlerBuilder> Map)
{
    public static EndpointMethod Post => new(
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapPost(route, handler)
    );

    public static EndpointMethod Put => new(
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapPut(route, handler)
    );

    public static EndpointMethod Get => new(
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapGet(route, handler)
    );

    public static EndpointMethod Delete => new(
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapDelete(route, handler)
    );

    public static EndpointMethod Patch => new(
        (RouteGroupBuilder routeGroupBuilder, string route, Delegate handler)
            => routeGroupBuilder.MapPatch(route, handler)
    );
}
