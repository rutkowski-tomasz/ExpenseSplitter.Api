using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.UnitTests;

public class ApplicationDbContextTests
{
    private readonly Mock<IPublisher> mockPublisher;
    private readonly TestApplicationDbContext context;

    public ApplicationDbContextTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        mockPublisher = new Mock<IPublisher>();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        context = new TestApplicationDbContext(options, mockPublisher.Object, dateTimeProviderMock.Object);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPublishDomainEvents()
    {
        var entity = new TestEntity(Guid.NewGuid());
        context.Add(entity);

        await context.SaveChangesAsync();
        
        mockPublisher.Verify(x => x.Publish(
            It.IsAny<IDomainEvent>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
        mockPublisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldClearsDomainEvents()
    {
        var entity = new TestEntity(Guid.NewGuid());
        context.Add(entity);

        await context.SaveChangesAsync();

        entity.GetOnSaveDomainEvents().Should().BeEmpty();
    }
    
    
    private class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id)
        {
            AddOnSaveDomainEvent(new TestDomainEvent());
        }
    }

    private record TestDomainEvent : IDomainEvent;
    
    private class TestApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider
    ) : ApplicationDbContext(options, publisher, dateTimeProvider)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TestEntity>();
        }
    }
}
