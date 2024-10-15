namespace ExpenseSplitter.Api.Domain.Abstractions;

public abstract class Entity<TEntityId>(TEntityId id) : IEntity
{
    private readonly List<DomainEvent> domainEvents = [];

    public TEntityId Id { get; init; } = id;
    public DateTime LastModified { get; set; }

    public IReadOnlyList<DomainEvent> GetPersistDomainEvents()
    {
        return domainEvents.ToList();
    }

    public void ClearPersistDomainEvents()
    {
        domainEvents.Clear();
    }

    protected void AddPersistDomainEvent(DomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
