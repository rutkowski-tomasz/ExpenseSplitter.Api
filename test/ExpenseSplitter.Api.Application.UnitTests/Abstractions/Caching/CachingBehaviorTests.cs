using ExpenseSplitter.Api.Application.Abstractions.Caching;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.UnitTests.Abstractions.Caching;

public class CachingBehaviorTests
{
    private readonly Mock<ICacheService> cacheService;
    private readonly CachingBehavior<TestQuery, Result<int>> behavior;

    public CachingBehaviorTests()
    {
        cacheService = new Mock<ICacheService>();
        behavior = new CachingBehavior<TestQuery, Result<int>>(cacheService.Object);
    }

    private record TestQuery : ICachedQuery
    {
        public string Key => "test";
        public TimeSpan? Expiration => null;
    }

    [Fact]
    public async Task Handle_ShouldUseCachedResult_WhenPresent()
    {
        var cachedResult = Result.Success(2137);
        cacheService
            .Setup(x => x.GetOrCreateAsync(
                It.Is<string>(y => y == "test"),
                It.IsAny<Func<CancellationToken, Task<Result<int>>>>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(cachedResult);

        var next = new RequestHandlerDelegate<Result<int>>(() => Task.FromResult(Result.Success(1)));
        var result = await behavior.Handle(new TestQuery(), next, default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(2137);
    }
}
