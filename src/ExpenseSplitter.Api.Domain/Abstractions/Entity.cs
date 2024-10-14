namespace ExpenseSplitter.Api.Domain.Abstractions;

public abstract class Entity<TEntityId>(TEntityId id) : IEntity
{
    private readonly List<IDomainEvent> domainEvents = [];

    public TEntityId Id { get; init; } = id;
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
