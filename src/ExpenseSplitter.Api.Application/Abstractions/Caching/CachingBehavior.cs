using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using MediatR;

namespace ExpenseSplitter.Api.Application.Abstractions.Caching;

internal sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
{
    private readonly ICacheService cacheService;

    public CachingBehavior(ICacheService cacheService)
    {
        this.cacheService = cacheService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var key = request.Key;
        var cachedResponse = await cacheService.GetOrCreate(
            key,
            _ => next(),
            request.Expiration,
            cancellationToken
        );

        return cachedResponse;
    }
}
