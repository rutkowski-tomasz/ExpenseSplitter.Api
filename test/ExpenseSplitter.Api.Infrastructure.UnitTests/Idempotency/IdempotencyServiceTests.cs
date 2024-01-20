using ExpenseSplitter.Api.Infrastructure.Idempotency;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ExpenseSplitter.Api.Infrastructure.UnitTests.Idempotency;

public class IdempotencyServiceTests
{
    private readonly Mock<IHttpContextAccessor> contextAccessorMock;
    private readonly IdempotencyService idempotencyService;

    public IdempotencyServiceTests()
    {
        contextAccessorMock = new Mock<IHttpContextAccessor>();

        idempotencyService = new IdempotencyService(
            TestApplicationDbContext.Create(),
            contextAccessorMock.Object
        );
    }

    [Theory]
    [InlineData("4b091b09-7e1e-4e3f-b1da-907a908ca98f", true)]
    [InlineData(null, false)]
    [InlineData("", true)]
    public void IsIdempotencyKeyInHeaders_ShouldCheckIdempotencyKeyFromHeaders(string? key, bool expectedValue)
    {
        SetupIdempotencyKey(key);

        var isInHeaders = idempotencyService.IsIdempotencyKeyInHeaders();

        isInHeaders.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("4b091b09-7e1e-4e3f-b1da-907a908ca98f", true)]
    [InlineData("", false)]
    [InlineData("test", false)]
    public void TryParseIdempotencyKey_ShouldProcess(string? key, bool expectedIsParsed)
    {
        SetupIdempotencyKey(key);

        var isParsed = idempotencyService.TryParseIdempotencyKey(out var _);

        isParsed.Should().Be(expectedIsParsed);
    }

    private void SetupIdempotencyKey(string? key)
    {
        var keyValueDictionary = new Dictionary<string, StringValues>();

        if (key is not null)
        {
            keyValueDictionary.Add("X-Idempotency-Key", key);
        }

        var headerDictionary = new HeaderDictionary(keyValueDictionary);
        contextAccessorMock.Setup(x => x.HttpContext!.Request.Headers).Returns(headerDictionary);
    }
}
