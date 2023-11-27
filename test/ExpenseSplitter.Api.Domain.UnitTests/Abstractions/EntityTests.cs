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
        domainEvents.First().Should().BeOfType<TestDomainEvent>();
    }

    [Fact]
    public void ClearDomainEvents_ShouldClearDomainEventsList()
    {
        var guid = new Fixture().Create<Guid>();
        var entity = new TestEntity(guid);

        entity.RaiseTestDomainEvent();
        entity.ClearDomainEvents();
        var domainEvents = entity.GetDomainEvents();
        
        domainEvents.Should().BeEmpty();
    }
    
    
    private class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id)
        {
        }

        public void RaiseTestDomainEvent()
        {
            RaiseDomainEvent(new TestDomainEvent());
        }
    }

    private record TestDomainEvent : IDomainEvent;
}
