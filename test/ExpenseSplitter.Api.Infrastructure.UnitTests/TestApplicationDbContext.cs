using ExpenseSplitter.Api.Application.Abstractions.Clock;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Infrastructure.UnitTests;

internal class TestApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IPublisher publisher,
    IDateTimeProvider dateTimeProvider
) : ApplicationDbContext(options, publisher, dateTimeProvider)
{
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
