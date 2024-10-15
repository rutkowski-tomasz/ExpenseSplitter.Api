using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.MediatrEndpoints;

public abstract class Endpoint<TRequest, TCommand>(
    EndpointDefinition endpointDefinition,
    Func<TRequest, TCommand> mapRequest,
    Action<RouteHandlerBuilder>? routeHandlerCustomization = null
) : EndpointBase(endpointDefinition, routeHandlerCustomization)
    where TCommand : IRequest<Result>
    where TRequest : notnull
{
    protected override Delegate Handle() =>
        ([AsParameters] TRequest request, ISender sender) => HandleCommand(sender, mapRequest(request));
}

public abstract class Endpoint<TRequest, TCommand, TCommandResult, TResponse>(
    EndpointDefinition endpointDefinition,
    Func<TRequest, TCommand> mapRequest,
    Func<TCommandResult, TResponse> mapResponse,
    Action<RouteHandlerBuilder>? routeHandlerCustomization = null
) : EndpointBase(endpointDefinition, routeHandlerCustomization)
    where TCommand : IRequest<Result<TCommandResult>>
    where TRequest : notnull
{
    protected override Delegate Handle() =>
        ([AsParameters] TRequest request, ISender sender) => HandleCommand(sender, mapRequest(request), mapResponse);
}

public abstract class Endpoint<TCommand, TCommandResult, TResponse>(
    EndpointDefinition endpointDefinition,
    Func<TCommandResult, TResponse> mapResponse,
    Action<RouteHandlerBuilder>? routeHandlerCustomization = null
) : EndpointBase(endpointDefinition, routeHandlerCustomization)
    where TCommand : IRequest<Result<TCommandResult>>, new()
{
    protected override Delegate Handle() =>
        (ISender sender) => HandleCommand(sender, new TCommand(), mapResponse);
}
