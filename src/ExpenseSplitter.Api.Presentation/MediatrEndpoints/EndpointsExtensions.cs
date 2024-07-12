using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public static class EndpointsExtensions
{
    public static Endpoints Get(this Endpoints endpoints, [StringSyntax("Route")] string route)
        => endpoints with { Route = route, Map = Endpoints.MapGet };

    public static Endpoints Post(this Endpoints endpoints, [StringSyntax("Route")] string route)
        => endpoints with { Route = route, Map = Endpoints.MapPost };

    public static Endpoints Put(this Endpoints endpoints, [StringSyntax("Route")] string route)
        => endpoints with { Route = route, Map = Endpoints.MapPut };
    
    public static Endpoints Patch(this Endpoints endpoints, [StringSyntax("Route")] string route)
        => endpoints with { Route = route, Map = Endpoints.MapPatch };

    public static Endpoints Delete(this Endpoints endpoints, [StringSyntax("Route")] string route)
        => endpoints with { Route = route, Map = Endpoints.MapDelete };
}
