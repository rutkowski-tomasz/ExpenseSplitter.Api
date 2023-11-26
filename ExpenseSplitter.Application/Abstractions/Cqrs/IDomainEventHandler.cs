using ExpenseSplitter.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Application.Abstractions.Cqrs;

public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
     where TDomainEvent : IDomainEvent
{
}
