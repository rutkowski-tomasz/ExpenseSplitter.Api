namespace ExpenseSplitter.Presentation.Api.Middleware;

public class TraceIdMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<TraceIdMiddleware> logger;

    private const string TraceIdHeader = "traceId";

    public TraceIdMiddleware(
        RequestDelegate next,
        ILogger<TraceIdMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var hasTraceId = context.Request.Headers.TryGetValue(TraceIdHeader, out var traceId);

        if (!hasTraceId)
        {
            traceId = Guid.NewGuid().ToString();
            logger.LogTrace("No Trace ID found in the request headers. Generated new Trace ID: {TraceId}", traceId.ToString());
        }
        else
        {
            logger.LogTrace("Trace ID found in the request headers: {TraceId}", traceId.ToString());
        }

        context.Items[TraceIdHeader] = traceId;
        context.Response.Headers.TryAdd(TraceIdHeader, traceId);

        await next(context);
    }
}
