using System.Diagnostics.CodeAnalysis;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public static class EndpointDefinitionExtensions
{
    public static EndpointDefinition Get(this EndpointDefinition endpointDefinition, [StringSyntax("Route")] string route)
        => endpointDefinition with { Route = route, Map = EndpointDefinition.MapGet };

    public static EndpointDefinition Post(this EndpointDefinition endpointDefinition, [StringSyntax("Route")] string route)
        => endpointDefinition with { Route = route, Map = EndpointDefinition.MapPost };

    public static EndpointDefinition Put(this EndpointDefinition endpointDefinition, [StringSyntax("Route")] string route)
        => endpointDefinition with { Route = route, Map = EndpointDefinition.MapPut };

    public static EndpointDefinition Patch(this EndpointDefinition endpointDefinition, [StringSyntax("Route")] string route)
        => endpointDefinition with { Route = route, Map = EndpointDefinition.MapPatch };

    public static EndpointDefinition Delete(this EndpointDefinition endpointDefinition, [StringSyntax("Route")] string route)
        => endpointDefinition with { Route = route, Map = EndpointDefinition.MapDelete };
}
