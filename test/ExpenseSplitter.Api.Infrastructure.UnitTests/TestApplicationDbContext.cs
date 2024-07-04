using ExpenseSplitter.Api.Application.Abstraction.Clock;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.UnitTests;

internal class TestApplicationDbContext : ApplicationDbContext
{
    public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher, IDateTimeProvider dateTimeProvider)
        : base(options, publisher, dateTimeProvider)
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
        var dateTimeProvider = new Mock<IDateTimeProvider>();

        var context = new TestApplicationDbContext(
            options,
            mockPublisher.Object,
            dateTimeProvider.Object
        );
        return context;
    }
}
