using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public abstract class Endpoint<TRequest, TCommand>(
    EndpointDefinition EndpointDefinition,
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
    EndpointDefinition EndpointDefinition,
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
    EndpointDefinition EndpointDefinition,
    Func<TCommandResult, TResponse> MapResponse,
    Action<RouteHandlerBuilder>? RouteHandlerCustomization = null
) : EndpointBase(EndpointDefinition, RouteHandlerCustomization)
    where TCommand : IRequest<Result<TCommandResult>>, new()
{
    public override Delegate Handle() =>
        (ISender sender) => HandleCommand(sender, new TCommand(), MapResponse);
}
