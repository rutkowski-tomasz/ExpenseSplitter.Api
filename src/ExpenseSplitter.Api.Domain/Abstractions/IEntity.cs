namespace ExpenseSplitter.Api.Domain.Abstractions;

public interface IEntity
{
    DateTime LastModified { get; set; }

    IReadOnlyList<IDomainEvent> GetOnSaveDomainEvents();

    void ClearOnSaveDomainEvents();
}
