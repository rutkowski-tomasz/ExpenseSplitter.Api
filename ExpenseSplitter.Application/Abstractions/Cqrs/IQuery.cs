using ExpenseSplitter.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Application.Abstractions.Cqrs;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}