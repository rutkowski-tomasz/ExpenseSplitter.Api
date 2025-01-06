using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ExpenseSplitter.Api.Infrastructure.UnitTests;

public sealed class ApplicationDbContextTests : IDisposable
{
    private readonly IPublisher publisher = Substitute.For<IPublisher>();
    private readonly TestApplicationDbContext context;

    public ApplicationDbContextTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();

        context = new TestApplicationDbContext(options, publisher, dateTimeProvider);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPublishDomainEvents()
    {
        var entity = new TestEntity(Guid.CreateVersion7());
        context.Add(entity);

        await context.SaveChangesAsync();

        await publisher.Received(1).Publish(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>()); 
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldClearsDomainEvents()
    {
        var entity = new TestEntity(Guid.CreateVersion7());
        context.Add(entity);

        await context.SaveChangesAsync();

        entity.GetOnSaveDomainEvents().Should().BeEmpty();
    }
    
    
    private sealed class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id)
        {
            AddOnSaveDomainEvent(new TestDomainEvent());
        }
    }

    private sealed record TestDomainEvent : IDomainEvent;
    
    private sealed class TestApplicationDbContext(
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

    public void Dispose()
    {
        context.Dispose();
    }
}
