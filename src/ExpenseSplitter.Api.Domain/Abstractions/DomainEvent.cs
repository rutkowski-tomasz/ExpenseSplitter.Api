using MediatR;

namespace ExpenseSplitter.Api.Domain.Abstractions;

public abstract record DomainEvent : INotification;
