using ExpenseSplitter.Api.Application.Abstractions.Clock;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

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

        var publisher = Substitute.For<IPublisher>();
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();

        var context = new TestApplicationDbContext(
            options,
            publisher,
            dateTimeProvider
        );
        return context;
    }
}
