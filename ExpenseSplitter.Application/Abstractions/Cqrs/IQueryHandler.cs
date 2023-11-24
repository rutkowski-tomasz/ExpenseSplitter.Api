using ExpenseSplitter.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Application.Abstractions.Cqrs;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}