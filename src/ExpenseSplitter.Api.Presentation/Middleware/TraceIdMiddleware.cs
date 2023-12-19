using Serilog.Context;

namespace ExpenseSplitter.Api.Presentation.Middleware;

public class TraceIdMiddleware
{
    private readonly RequestDelegate next;

    private const string TraceIdHeader = "traceId";

    public TraceIdMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = GetTraceId(context);
        context.Items[TraceIdHeader] = traceId;
        context.Response.Headers.TryAdd(TraceIdHeader, traceId);

        using (LogContext.PushProperty("TraceId", traceId))
        {
            await next(context);
        }
    }

    private string GetTraceId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(TraceIdHeader, out var traceId);
        return traceId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
