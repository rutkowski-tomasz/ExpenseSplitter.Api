using System.Runtime.InteropServices.JavaScript;
using ExpenseSplitter.Api.Presentation.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Presentation.Tests.Middleware;

public class TraceIdMiddlewareTests
{
    private readonly Mock<RequestDelegate> nextMock;
    private readonly Mock<ILogger<TraceIdMiddleware>> loggerMock;
    private readonly TraceIdMiddleware middleware;

    public TraceIdMiddlewareTests()
    {
        nextMock = new Mock<RequestDelegate>();
        loggerMock = new Mock<ILogger<TraceIdMiddleware>>();
        middleware = new TraceIdMiddleware(nextMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_ShouldGenerateTraceId_WhenNoTraceIdIsPresentInRequestHeaders()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Clear();

        await middleware.InvokeAsync(context);

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

        await middleware.InvokeAsync(context);

        context.Items["traceId"].Should().Be(existingTraceId);
        context.Response.Headers["traceId"].ToArray().Should().BeEquivalentTo(existingTraceId);
    }
}
