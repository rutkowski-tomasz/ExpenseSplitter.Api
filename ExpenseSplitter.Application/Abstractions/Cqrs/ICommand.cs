using ExpenseSplitter.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Application.Abstractions.Cqrs;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}

public interface ICommand<TReponse> : IRequest<Result<TReponse>>, IBaseCommand
{
}

public interface IBaseCommand
{
}