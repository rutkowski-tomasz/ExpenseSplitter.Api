using ExpenseSplitter.Api.Application.Abstractions.Caching;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.UnitTests.Abstractions.Caching;

public class CachingBehaviorTests
{
    private readonly ICacheService cacheService;
    private readonly CachingBehavior<TestQuery, Result<int>> behavior;

    public CachingBehaviorTests()
    {
        cacheService = Substitute.For<ICacheService>();
        behavior = new CachingBehavior<TestQuery, Result<int>>(cacheService);
    }

    private sealed record TestQuery : ICachedQuery
    {
        public string Key => "test";
        public TimeSpan? Expiration => null;
    }

    [Fact]
    public async Task Handle_ShouldUseCachedResult_WhenPresent()
    {
        var cachedResult = Result.Success(2137);
        cacheService.GetOrCreate(
            Arg.Is<string>(x => x == "test"),
            Arg.Any<Func<CancellationToken, Task<Result<int>>>>(),
            Arg.Any<TimeSpan?>(),
            Arg.Any<CancellationToken>()
        ).Returns(cachedResult);

        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(1)));
        var result = await behavior.Handle(new TestQuery(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2137);
    }
}
