using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.Abstractions.Cqrs;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
     where TDomainEvent : IDomainEvent;
