using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.UnitTests;

internal class TestApplicationDbContext : ApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher)
        : base(options, publisher)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var mockPublisher = new Mock<IPublisher>();

        var context = new TestApplicationDbContext(options, mockPublisher.Object);
        return context;
    }
}