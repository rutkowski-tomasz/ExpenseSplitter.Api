namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

public interface IIdempotencyService
{
    bool IsIdempotencyKeyInHeaders();
    bool TryParseIdempotencyKey(out Guid o);
    Task<bool> IsIdempotencyKeyProcessed(Guid parsedIdempotencyKey, CancellationToken cancellationToken);
    Task SaveIdempotencyKey(Guid parsedIdempotencyKey, string name, CancellationToken cancellationToken);
}

