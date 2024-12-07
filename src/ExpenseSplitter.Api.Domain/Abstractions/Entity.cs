namespace ExpenseSplitter.Api.Domain.Abstractions;

public abstract class Entity<TEntityId>(TEntityId id) : IEntity
{
    private readonly List<IDomainEvent> domainEvents = [];

    public TEntityId Id { get; init; } = id;
    public DateTime LastModified { get; set; }

    public IReadOnlyList<IDomainEvent> GetOnSaveDomainEvents()
    {
        return [.. domainEvents];
    }

    public void ClearOnSaveDomainEvents()
    {
        domainEvents.Clear();
    }

    protected void AddOnSaveDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
