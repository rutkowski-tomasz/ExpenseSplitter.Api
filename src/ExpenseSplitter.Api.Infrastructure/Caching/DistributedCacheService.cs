using ExpenseSplitter.Api.Application.Abstractions.Caching;
using ExpenseSplitter.Api.Infrastructure.Serializer;
using Microsoft.Extensions.Caching.Distributed;

namespace ExpenseSplitter.Api.Infrastructure.Caching;

public class DistributedCacheService(IDistributedCache distributedCache, ISerializer serializer) : ICacheService
{
    private static readonly DistributedCacheEntryOptions DefaultCacheEntryOptions
        = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };
    
    public async Task<T> GetOrCreate<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var serializedBytes = await distributedCache.GetAsync(key, cancellationToken);
        if (IsCacheHit(serializedBytes))
        {
            var deserialized = serializer.Deserialize<T>(serializedBytes!, cancellationToken);
            return deserialized;
        }
        
        var result = await factory(cancellationToken);
        var cacheEntryOptions = GetDistributedCacheEntryOptions(expiration);
        await CacheSet(key, result, cacheEntryOptions, cancellationToken);

        return result;
    }
    
    public async Task<Dictionary<string, T>> BatchGetOrCreate<T>(
        List<string> keys,
        Func<List<string>, CancellationToken, Task<Dictionary<string, T>>> factoryFunction,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    )
    {
        var cacheLookupTasks = keys.Select(x =>
            distributedCache.GetAsync(x, cancellationToken)
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
        var serializedBytes = await distributedCache.GetAsync(key, cancellationToken);
        if (!IsCacheHit(serializedBytes))
        {
            return (false, default);
        }
 
        var value = serializer.Deserialize<T>(serializedBytes!, cancellationToken);
        return (true, value);
    }
    
    public Task Set<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cacheEntryOptions = GetDistributedCacheEntryOptions(expiration);
        return CacheSet(key, value, cacheEntryOptions, cancellationToken);
    }

    private static bool IsCacheHit(byte[]? bytes)
    {
        return bytes is not null && bytes.Length > 0;
    }
    
    private static DistributedCacheEntryOptions GetDistributedCacheEntryOptions(TimeSpan? expiration = null)
    {
        return expiration is not null
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
            : DefaultCacheEntryOptions;
    }
    
    private Task CacheSet<T>(string key, T value, DistributedCacheEntryOptions cacheEntryOptions, CancellationToken cancellationToken)
    {
        var serializedResult = serializer.Serialize(value, cancellationToken);
        return distributedCache.SetAsync(key, serializedResult, cacheEntryOptions, cancellationToken);
    }
}
