using ExpenseSplitter.Api.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Hybrid;

namespace ExpenseSplitter.Api.Infrastructure.Caching;

public class HybridCacheService(HybridCache cache) : ICacheService
{
    private const double LocalCacheMultiplier = 0.2;

    private static readonly HybridCacheEntryOptions DefaultCacheEntryOptions
        = new()
        {
            LocalCacheExpiration = TimeSpan.FromMinutes(5) * LocalCacheMultiplier,
            Expiration = TimeSpan.FromMinutes(5)
        };
    
    public async Task<T> GetOrCreate<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = expiration
        };
        
        var result = await cache.GetOrCreateAsync(key, factory, options, cancellationToken: cancellationToken);

        return result;
    }

    public Task<Dictionary<string, T>> BatchGetOrCreate<T>(
        List<string> keys,
        Func<List<string>, CancellationToken, Task<Dictionary<string, T>>> factoryFunction,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var cacheLookupTasks = keys.Select(x =>
            TryGet(x, cancellationToken)
        );
        var cacheLookupResults = await Task.WhenAll(cacheLookupTasks);

        var keysToResolve = new List<string>();
        var results = new Dictionary<string, T>();

        for (var i = 0; i < keys.Count; i += 1)
        {
            var cacheLookupResult = cacheLookupResults[i];
            if (!IsCacheHit(cacheLookupResult))
            {
                keysToResolve.Add(keys[i]);
                continue;
            }
            
            var deserialized = serializer.Deserialize<T>(cacheLookupResult!, cancellationToken);
            results.Add(keys[i], deserialized);
        }
        
        if (keysToResolve.Count == 0)
        {
            return results;
        }
        
        var resolvedKeys = await factoryFunction(keysToResolve, cancellationToken);
        foreach (var (key, value) in resolvedKeys)
        {
            results.Add(key, value);
        }

        var cacheEntryOptions = GetDistributedCacheEntryOptions(expiration);
        var cacheSetTasks = resolvedKeys.Select(x => CacheSet(x.Key, x.Value, cacheEntryOptions, cancellationToken));
        await Task.WhenAll(cacheSetTasks);

        return results;
    }

    public async Task<(bool isFound, T? result)> TryGet<T>(string key, CancellationToken cancellationToken)
    {
        var result = await cache.GetOrCreateAsync(
            key,
            _ => ValueTask.FromResult<T?>(default),
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.Zero,
                LocalCacheExpiration = TimeSpan.Zero
            },
            cancellationToken: cancellationToken
        );

        return result is not null
            ? (false, default)
            : (true, result);
    }
    
    public Task Set<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = GetDistributedCacheEntryOptions(expiration);
        return CacheSet(key, value, cacheEntryOptions, cancellationToken);
    }
    
    private static HybridCacheEntryOptions GetDistributedCacheEntryOptions(TimeSpan? expiration = null)
    {
        return expiration is not null
            ? new HybridCacheEntryOptions
            {
                LocalCacheExpiration = expiration * LocalCacheMultiplier,
                Expiration = expiration
            }
            : DefaultCacheEntryOptions;
    }
    
    private async Task CacheSet<T>(string key, T value, HybridCacheEntryOptions options, CancellationToken cancellationToken)
    {
        await cache.SetAsync(key, value, options, cancellationToken: cancellationToken);
    }
}
