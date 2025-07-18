using ExpenseSplitter.Api.Presentation.Middleware;
using Microsoft.AspNetCore.Http;

namespace ExpenseSplitter.Api.Presentation.UnitTests.Middleware;

public class TraceIdMiddlewareTests
{
    private readonly TraceIdMiddleware middleware = new(Substitute.For<RequestDelegate>());

    [Fact]
    public async Task InvokeAsync_ShouldGenerateTraceId_WhenNoTraceIdIsPresentInRequestHeaders()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Clear();

        await middleware.InvokeAsync(context);

        context.Response.Headers.Should().ContainKey("traceId");

        var traceId = context.Response.Headers["traceId"].ToString();
        traceId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task InvokeAsync_ShouldReuseTraceId_WhenTraceIdIsPresentInRequestHeaders()
    {
        var existingTraceId = Guid.CreateVersion7().ToString();
        var context = new DefaultHttpContext
        {
            TraceIdentifier = existingTraceId
        };

        await middleware.InvokeAsync(context);

        context.Response.Headers["traceId"].ToArray().Should().BeEquivalentTo(existingTraceId);
    }
}
