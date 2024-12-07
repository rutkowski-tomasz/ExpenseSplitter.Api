namespace ExpenseSplitter.Api.Infrastructure.Idempotency;

internal sealed class IdempotentRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedOnUtc { get; init; }
}

