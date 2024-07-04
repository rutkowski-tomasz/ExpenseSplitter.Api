namespace ExpenseSplitter.Api.Domain.Abstractions;

public abstract class Entity<TEntityId> : IEntity
{
    private readonly List<IDomainEvent> domainEvents = new();

    protected Entity(TEntityId id)
    {
        Id = id;
    }

    public TEntityId Id { get; init; }
    public DateTime LastModified { get; set; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
