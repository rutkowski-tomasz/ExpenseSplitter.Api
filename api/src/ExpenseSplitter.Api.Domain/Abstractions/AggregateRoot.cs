namespace ExpenseSplitter.Api.Domain.Abstractions;

public abstract class AggregateRoot<TEntityId>(TEntityId id) : Entity<TEntityId>(id);
