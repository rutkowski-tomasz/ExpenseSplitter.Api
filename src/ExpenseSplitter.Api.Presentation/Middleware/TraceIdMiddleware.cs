using Serilog.Context;

namespace ExpenseSplitter.Api.Presentation.Middleware;

public class TraceIdMiddleware(RequestDelegate next)
{
    private const string TraceIdHeader = "traceId";

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

    private static string GetTraceId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(TraceIdHeader, out var traceId);
        return traceId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
