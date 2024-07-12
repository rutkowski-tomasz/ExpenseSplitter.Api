using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public partial class EndpointGroups;

public abstract class EndpointBase(
    Endpoints EndpointDefinition,
    Action<RouteHandlerBuilder>? RouteHandlerCustomization = null
) : EndpointGroups
{
    public abstract Delegate Handle();

    public RouteHandlerBuilder CreateRouteHandlerBuilder(RouteGroupBuilder routeGroupBuilder)
    {
        return EndpointDefinition.Map(routeGroupBuilder, EndpointDefinition.Route, Handle());
    }

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = EndpointDefinition.GroupBuilder(builder);
        var routeHandlerBuilder = CreateRouteHandlerBuilder(routeGroupBuilder);

        if (RouteHandlerCustomization is not null)
        {
            RouteHandlerCustomization(routeHandlerBuilder);
        }

        foreach (var statusCode in EndpointDefinition.ErrorCodes ?? [])
        {
            routeHandlerBuilder.Produces<string>(statusCode);
        }
    }

    public async Task<IResult> HandleCommand<TCommand, TCommandResult, TResponse>(
        ISender sender,
        TCommand command,
        Func<TCommandResult, TResponse> mapResponse
    ) where TCommand : IRequest<Result<TCommandResult>>
        => ToHttpResult(await sender.Send(command), mapResponse);
    
    public async Task<IResult> HandleCommand<TCommand>(ISender sender, TCommand command)
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

public abstract class Endpoint<TRequest, TCommand>(
    Endpoints EndpointDefinition,
    Func<TRequest, TCommand> MapRequest,
    Action<RouteHandlerBuilder>? RouteHandlerCustomization = null
) : EndpointBase(EndpointDefinition, RouteHandlerCustomization)
    where TCommand : IRequest<Result>
    where TRequest : notnull
{
    public override Delegate Handle() =>
        ([AsParameters] TRequest request, ISender sender) => HandleCommand(sender, MapRequest(request));
}

public abstract class Endpoint<TRequest, TCommand, TCommandResult, TResponse>(
    Endpoints EndpointDefinition,
    Func<TRequest, TCommand> MapRequest,
    Func<TCommandResult, TResponse> MapResponse,
    Action<RouteHandlerBuilder>? RouteHandlerCustomization = null
) : EndpointBase(EndpointDefinition, RouteHandlerCustomization)
    where TCommand : IRequest<Result<TCommandResult>>
    where TRequest : notnull
{
    public override Delegate Handle() =>
        ([AsParameters] TRequest request, ISender sender) => HandleCommand(sender, MapRequest(request), MapResponse);
}

public abstract class Endpoint<TCommand, TCommandResult, TResponse>(
    Endpoints EndpointDefinition,
    Func<TCommandResult, TResponse> MapResponse,
    Action<RouteHandlerBuilder>? RouteHandlerCustomization = null
) : EndpointBase(EndpointDefinition, RouteHandlerCustomization)
    where TCommand : IRequest<Result<TCommandResult>>, new()
{
    public override Delegate Handle() =>
        (ISender sender) => HandleCommand(sender, new TCommand(), MapResponse);
}
