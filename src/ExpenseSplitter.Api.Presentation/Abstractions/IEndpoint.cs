using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Presentation.Abstractions;

public interface IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder);
}


public interface IEndpoint<TRequest, TCommand> : IMapper<TRequest, TCommand>
    where TCommand : IRequest<Result>
{
    public void MapEndpoint(IEndpointRouteBuilder builder);
}

public interface IEndpoint<TRequest, TCommand, TCommandResult, TResponse>
    : IMapper<TRequest, TCommand>, IMapper<TCommandResult, TResponse>
    where TCommand : IRequest<Result<TCommandResult>>
{
    public void MapEndpoint(IEndpointRouteBuilder builder);
}
