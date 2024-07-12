using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public static class EndpointDefinitionExtensions
{
    public static EndpointDefinition Get(this EndpointDefinition EndpointDefinition, [StringSyntax("Route")] string route)
        => EndpointDefinition with { Route = route, Map = EndpointDefinition.MapGet };

    public static EndpointDefinition Post(this EndpointDefinition EndpointDefinition, [StringSyntax("Route")] string route)
        => EndpointDefinition with { Route = route, Map = EndpointDefinition.MapPost };

    public static EndpointDefinition Put(this EndpointDefinition EndpointDefinition, [StringSyntax("Route")] string route)
        => EndpointDefinition with { Route = route, Map = EndpointDefinition.MapPut };

    public static EndpointDefinition Patch(this EndpointDefinition EndpointDefinition, [StringSyntax("Route")] string route)
        => EndpointDefinition with { Route = route, Map = EndpointDefinition.MapPatch };

    public static EndpointDefinition Delete(this EndpointDefinition EndpointDefinition, [StringSyntax("Route")] string route)
        => EndpointDefinition with { Route = route, Map = EndpointDefinition.MapDelete };
}
