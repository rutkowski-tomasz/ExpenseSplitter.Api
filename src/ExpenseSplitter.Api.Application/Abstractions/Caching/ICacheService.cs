using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Application.Abstractions.Caching;

public interface ICacheService
{
    Task<T> GetOrCreate<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    );

    Task<Dictionary<string, T>> BatchGetOrCreate<T>(
        List<string> keys,
        Func<List<string>, CancellationToken, Task<Dictionary<string, T>>> factoryFunction,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default
    );
    
    Task<(bool isFound, T? result)> TryGet<T>(string key, CancellationToken cancellationToken);
    
    Task Set<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
}
