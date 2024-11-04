using ExpenseSplitter.Api.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace ExpenseSplitter.Api.Infrastructure.Caching;

internal sealed class InMemoryCacheService(IMemoryCache cache) : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    public async Task<T> GetOrCreate<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var result = await cache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(expiration ?? DefaultExpiration);
                return factory(cancellationToken);
            });

        return result!;
    }
    
    public async Task<Dictionary<string, T>> BatchGetOrCreate<T>(
        List<string> keys,
        Func<List<string>, CancellationToken, Task<Dictionary<string, T>>> factoryFunction,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var results = new Dictionary<string, T>();
        var keysToResolve = new List<string>();

        foreach (var key in keys)
        {
            var isFound = cache.TryGetValue<T>(key, out var value);
            if (!isFound)
            {
                keysToResolve.Add(key);
            }

            results.Add(key, value!);
        }
        
        if (keysToResolve.Count == 0)
        {
            return results;
        }
        
        var resolvedKeys = await factoryFunction(keysToResolve, cancellationToken);
        foreach (var (key, value) in resolvedKeys)
        {
            results.Add(key, value);
            cache.Set(key, value, expiration ?? DefaultExpiration);
        }

        return results;
    }

    public Task<(bool isFound, T? result)> TryGet<T>(string key, CancellationToken _)
    {
        var isFound = cache.TryGetValue<T>(key, out var result);
        return Task.FromResult((isFound, result));
    }

    public Task Set<T>(string key, T value, TimeSpan? expiration = null, CancellationToken _ = default)
    {
        cache.Set(key, value, expiration ?? DefaultExpiration);
        return Task.CompletedTask;
    }
}
