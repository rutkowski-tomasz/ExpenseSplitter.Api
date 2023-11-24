using ExpenseSplitter.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Application.Abstractions.Cqrs;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}