using ExpenseSplitter.Api.Application.Abstractions.Caching;
using ExpenseSplitter.Api.Infrastructure.Idempotency;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace ExpenseSplitter.Api.Infrastructure.UnitTests.Idempotency;

public class IdempotencyServiceTests
{
    private readonly IHttpContextAccessor contextAccessor = Substitute.For<IHttpContextAccessor>();
    private readonly ICacheService cacheService = Substitute.For<ICacheService>();
    private readonly IdempotencyService idempotencyService;

    public IdempotencyServiceTests()
    {
        idempotencyService = new IdempotencyService(
            cacheService,
            contextAccessor
        );
    }

    [Theory]
    [InlineData("4b091b09-7e1e-4e3f-b1da-907a908ca98f", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("a", false)]
    public void IsIdempotencyKeyInHeaders_ShouldCheckIdempotencyKeyFromHeaders(string? key, bool expectedValue)
    {
        SetupIdempotencyKey(key);

        var isInHeaders = idempotencyService.GetIdempotencyKeyFromHeaders();

        isInHeaders.IsSuccess.Should().Be(expectedValue);
    }

    private void SetupIdempotencyKey(string? key)
    {
        contextAccessor.HttpContext = new DefaultHttpContext();
        contextAccessor.HttpContext.Request.Headers.Append("X-Idempotency-Key", key!);
    }
}
