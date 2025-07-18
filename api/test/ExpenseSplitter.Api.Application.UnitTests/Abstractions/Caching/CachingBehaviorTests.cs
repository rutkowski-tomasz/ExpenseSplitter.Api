using ExpenseSplitter.Api.Application.Abstractions.Caching;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.UnitTests.Abstractions.Caching;

public class CachingBehaviorTests
{
    private readonly ICacheService cacheService;
    private readonly CachingBehavior<TestQuery, Result<int>> behavior;
    private readonly TestQuery request;

    public CachingBehaviorTests()
    {
        cacheService = Substitute.For<ICacheService>();
        behavior = new CachingBehavior<TestQuery, Result<int>>(cacheService);
        request = new TestQuery();
    }

    public record TestQuery : ICachedQuery
    {
        public string Key => "test-key";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedValue_WhenCacheContainsValue()
    {
        var cachedValue = Result.Success(42);
        cacheService.GetOrCreate(
            request.Key,
            Arg.Any<Func<CancellationToken, Task<Result<int>>>>(),
            request.Expiration,
            Arg.Any<CancellationToken>()
        ).Returns(cachedValue);

        var next = new RequestHandlerDelegate<Result<int>>(cancellationToken => Task.FromResult(Result.Success(100)));

        var result = await behavior.Handle(request, next, default);

        result.Should().Be(cachedValue);
        await cacheService.Received(1).GetOrCreate(
            request.Key,
            Arg.Any<Func<CancellationToken, Task<Result<int>>>>(),
            request.Expiration,
            Arg.Any<CancellationToken>()
        );
    }
}
