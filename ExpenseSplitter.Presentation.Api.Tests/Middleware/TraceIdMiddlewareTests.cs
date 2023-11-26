using ExpenseSplitter.Presentation.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Presentation.Api.Tests.Middleware;

public class TraceIdMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<TraceIdMiddleware>> _loggerMock;
    private readonly TraceIdMiddleware _middleware;

    public TraceIdMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<TraceIdMiddleware>>();
        _middleware = new TraceIdMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_ShouldGenerateTraceId_WhenNoTraceIdIsPresentInRequestHeaders()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Clear();

        await _middleware.InvokeAsync(context);

        context.Items.Should().ContainKey("traceId");
        context.Response.Headers.Should().ContainKey("traceId");

        var generatedTraceId = context.Response.Headers["traceId"].ToString();
        Guid.TryParse(generatedTraceId, out var _).Should().BeTrue("Generated traceId should be a valid GUID");
    }

    [Fact]
    public async Task InvokeAsync_ShouldReuseTraceId_WhenTraceIdIsPresentInRequestHeaders()
    {
        var context = new DefaultHttpContext();
        var existingTraceId = Guid.NewGuid().ToString();
        context.Request.Headers["traceId"] = existingTraceId;

        await _middleware.InvokeAsync(context);

        context.Items["traceId"].Should().Be(existingTraceId);
        context.Response.Headers["traceId"].Should().BeEquivalentTo(new List<string> { existingTraceId });
    }
}
