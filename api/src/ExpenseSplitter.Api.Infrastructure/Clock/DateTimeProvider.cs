using ExpenseSplitter.Api.Application.Abstractions.Clock;

namespace ExpenseSplitter.Api.Infrastructure.Clock;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
