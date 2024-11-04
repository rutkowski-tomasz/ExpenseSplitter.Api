using ExpenseSplitter.Api.Application.Abstractions.Caching;
using ExpenseSplitter.Api.Application.Abstractions.Idempotency;
using ExpenseSplitter.Api.Domain.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ExpenseSplitter.Api.Infrastructure.Idempotency;

internal sealed class IdempotencyService(
    ICacheService cacheService,
    IHttpContextAccessor accessor
) : IIdempotencyService
{
    private const string IdempotencyKeyHeaderName = "X-Idempotency-Key";
    private readonly AppError noIdempotencyKey = new(ErrorType.NotFound, "Idempotency key not found in headers");
    private readonly AppError idempotencyKeyNotAGuid = new(ErrorType.PreConditionFailed, "Idempotency key is not a valid GUID");
    private readonly TimeSpan idempotencyExpiration = TimeSpan.FromHours(24);

    public Result<Guid> GetIdempotencyKeyFromHeaders()
    {
        var headers = accessor.HttpContext?.Request.Headers;
        var idempotencyKey = headers![IdempotencyKeyHeaderName].FirstOrDefault();

        if (idempotencyKey is null)
        {
            return noIdempotencyKey;
        }

        return Guid.TryParse(idempotencyKey, out var parsedIdempotencyKey)
            ? parsedIdempotencyKey
            : idempotencyKeyNotAGuid;
    }

    public Task<(bool isProcessed, T? result)> GetProcessedRequest<T>(Guid parsedIdempotencyKey, CancellationToken cancellationToken)
    {
        return cacheService.TryGet<T>(BuildIdempotencyCacheKey(parsedIdempotencyKey), cancellationToken);
    }

    public Task SaveIdempotentRequest<T>(Guid parsedIdempotencyKey, T value, CancellationToken cancellationToken)
    {
        return cacheService.Set(BuildIdempotencyCacheKey(parsedIdempotencyKey), value, idempotencyExpiration, cancellationToken);
    }
    
    private static string BuildIdempotencyCacheKey(Guid parsedIdempotencyKey)
    {
        return $"IdempotentRequest_{parsedIdempotencyKey}";
    }
}
