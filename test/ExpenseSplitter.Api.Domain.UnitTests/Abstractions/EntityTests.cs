using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Domain.UnitTests.Abstractions;

public class EntityTests
{
    [Fact]
    public void GetDomainEvents_ShouldReturnEmptyCollection_WhenNoDomainEventsWereRaised()
    {
        var guid = new Fixture().Create<Guid>();
        var entity = new TestEntity(guid);

        var domainEvents = entity.GetDomainEvents();

        domainEvents.Should().BeEmpty();
    }

    [Fact]
    public void RaiseDomainEvent_ShouldAddEventToDomainEventsList()
    {
        var guid = new Fixture().Create<Guid>();
        var entity = new TestEntity(guid);

        entity.RaiseTestDomainEvent();
        var domainEvents = entity.GetDomainEvents();
        
        domainEvents.Should().HaveCount(1);
        domainEvents[0].Should().BeOfType<TestDomainEvent>();
    }

    [Fact]
    public void ClearDomainEvents_ShouldClearDomainEventsList()
    {
        var guid = new Fixture().Create<Guid>();
        var entity = new TestEntity(guid);

        entity.RaiseTestDomainEvent();
        entity.ClearOnSaveEvents();
        var domainEvents = entity.GetDomainEvents();
        
        domainEvents.Should().BeEmpty();
    }
    
    
    private class TestEntity(Guid id) : Entity<Guid>(id)
    {
        public void RaiseTestDomainEvent()
        {
            AddOnSaveEvent(new TestDomainEvent());
        }
    }

    private record TestDomainEvent : IDomainEvent;
}
