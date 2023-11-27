using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.Abstractions.Cqrs;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}