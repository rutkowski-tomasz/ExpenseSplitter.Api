namespace ExpenseSplitter.Api.Application.Abstraction.Clock;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}