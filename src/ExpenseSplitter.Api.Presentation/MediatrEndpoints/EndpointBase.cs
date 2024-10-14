using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public abstract class EndpointBase(
    EndpointDefinition endpointDefinition,
    Action<RouteHandlerBuilder>? routeHandlerCustomization = null
)
{
    protected abstract Delegate Handle();

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder = endpointDefinition.GroupBuilder(builder);
        var routeHandlerBuilder = endpointDefinition.Map(builder, endpointDefinition.Route, Handle());

        routeHandlerCustomization?.Invoke(routeHandlerBuilder);
        foreach (var statusCode in endpointDefinition.ErrorCodes)
        {
            routeHandlerBuilder.Produces<string>(statusCode);
        }
    }

    protected static async Task<IResult> HandleCommand<TCommand, TCommandResult, TResponse>(
        ISender sender,
        TCommand command,
        Func<TCommandResult, TResponse> mapResponse
    ) where TCommand : IRequest<Result<TCommandResult>>
        => ToHttpResult(await sender.Send(command), mapResponse);

    protected static async Task<IResult> HandleCommand<TCommand>(ISender sender, TCommand command)
        where TCommand : IRequest<Result>
        => ToHttpResult(await sender.Send(command));

    private static IResult ToHttpResult<TCommandResult, TResponse>(Result<TCommandResult> result, Func<TCommandResult, TResponse> mapResponse)
        => result.IsSuccess ? TypedResults.Ok(mapResponse(result.Value)) : MapResultError(result.Error);

    private static IResult ToHttpResult(Result result)
        => result.IsSuccess ? TypedResults.Ok() : MapResultError(result.Error);

    private static IResult MapResultError(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(error.Description),
            ErrorType.NotFound => Results.NotFound(error.Description),
            ErrorType.Forbidden => Results.Forbid(),
            ErrorType.BadRequest => Results.BadRequest(error.Description),
            ErrorType.PreConditionFailed => Results.Problem(error.Description, statusCode: StatusCodes.Status412PreconditionFailed),
            ErrorType.Conflict => Results.Conflict(error.Description),
            ErrorType.BadGateway => Results.Problem(error.Description, statusCode: StatusCodes.Status502BadGateway),
            ErrorType.Unauthorized => Results.Unauthorized(),
            ErrorType.NotModified => Results.StatusCode(StatusCodes.Status304NotModified),
            _ => Results.Problem(error.Description, statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
