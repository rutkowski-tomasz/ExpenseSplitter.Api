using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

public interface IIdempotencyService
{
    Result<Guid> GetIdempotencyKeyFromHeaders();
    Task<(bool isProcessed, T? result)> GetProcessedRequest<T>(Guid parsedIdempotencyKey, CancellationToken cancellationToken);
    Task SaveIdempotentRequest<T>(Guid parsedIdempotencyKey, T value, CancellationToken cancellationToken);
}
