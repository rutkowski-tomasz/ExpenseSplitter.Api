using ExpenseSplitter.Api.Application.Abstraction.Clock;

namespace ExpenseSplitter.Api.Infrastructure.Configurations;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
