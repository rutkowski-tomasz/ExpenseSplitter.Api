using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

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
