using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.Extensions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Abstractions;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder builder);
}

public abstract class Endpoint<TRequest, TCommand, TCommandResult, TResponse> : IEndpoint
    where TCommand : IRequest<Result<TCommandResult>>
{
    public abstract TCommand MapRequest(TRequest request);
    public abstract TResponse MapResponse(TCommandResult result);
    public abstract void MapEndpoint(IEndpointRouteBuilder builder);

    protected async Task<IResult> Handle(TRequest request, ISender sender)
    {
        var command = MapRequest(request);
        var result = await sender.Send(command);
        return result.ToHttpResult(MapResponse);
    }
}

public abstract class EndpointEmptyRequest<TCommand, TCommandResult, TResponse> : IEndpoint
    where TCommand : IRequest<Result<TCommandResult>>, new()
{
    public abstract TResponse MapResponse(TCommandResult result);
    public abstract void MapEndpoint(IEndpointRouteBuilder builder);

    protected async Task<IResult> Handle(ISender sender)
    {
        var command = new TCommand();
        var result = await sender.Send(command);
        return result.ToHttpResult(MapResponse);
    }
}

public abstract class EndpointEmptyResponse<TRequest, TCommand> : IEndpoint
    where TCommand : IRequest<Result>
{
    public abstract TCommand MapRequest(TRequest request);
    public abstract void MapEndpoint(IEndpointRouteBuilder builder);

    protected async Task<IResult> Handle(TRequest request, ISender sender)
    {
        var command = MapRequest(request);
        var result = await sender.Send(command);
        return result.ToHttpResult();
    }
}
