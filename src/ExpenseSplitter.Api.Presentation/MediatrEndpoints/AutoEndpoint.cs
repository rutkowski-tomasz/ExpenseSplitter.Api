namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

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
