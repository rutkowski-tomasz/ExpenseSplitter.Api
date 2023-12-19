using ExpenseSplitter.Api.Presentation.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Presentation.UnitTests.Middleware;

public class TraceIdMiddlewareTests
{
    private readonly TraceIdMiddleware middleware;

    public TraceIdMiddlewareTests()
    {
        var nextMock = new Mock<RequestDelegate>();
        var loggerMock = new Mock<ILogger<TraceIdMiddleware>>();
        middleware = new TraceIdMiddleware(nextMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_ShouldGenerateTraceId_WhenNoTraceIdIsPresentInRequestHeaders()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Clear();

        await middleware.InvokeAsync(context);

        context.Items.Should().ContainKey("traceId");
        context.Response.Headers.Should().ContainKey("traceId");

        var traceId = context.Response.Headers["traceId"].ToString();
        traceId.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task InvokeAsync_ShouldReuseTraceId_WhenTraceIdIsPresentInRequestHeaders()
    {
        var context = new DefaultHttpContext();
        var existingTraceId = Guid.NewGuid().ToString();
        context.Request.Headers["traceId"] = existingTraceId;

        await middleware.InvokeAsync(context);

        context.Items["traceId"].Should().Be(existingTraceId);
        context.Response.Headers["traceId"].ToArray().Should().BeEquivalentTo(existingTraceId);
    }
}
