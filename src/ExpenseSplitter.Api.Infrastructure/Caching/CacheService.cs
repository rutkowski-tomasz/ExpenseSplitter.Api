using ExpenseSplitter.Api.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace ExpenseSplitter.Api.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var result = await memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(expiration ?? DefaultExpiration);
                return factory(cancellationToken);
            });

        return result!;
    }
}
