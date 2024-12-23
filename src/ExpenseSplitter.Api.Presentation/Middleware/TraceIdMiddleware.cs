using System.Diagnostics;
using Serilog.Context;

namespace ExpenseSplitter.Api.Presentation.Middleware;

public class TraceIdMiddleware(RequestDelegate next)
{
    private const string TraceIdHeader = "traceId";
    private const string ActivityIdHeader = "activityId";
    private const string ActivityTraceIdHeader = "activityTraceId";

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.TraceIdentifier;
        context.Response.Headers.TryAdd(TraceIdHeader, traceId);

        var activityId = Activity.Current?.Id;
        context.Response.Headers.TryAdd(ActivityIdHeader, activityId);

        var activityTraceId = Activity.Current?.TraceId.ToString();
        context.Response.Headers.TryAdd(ActivityTraceIdHeader, activityTraceId);
        
        using (LogContext.PushProperty(TraceIdHeader, traceId))
        using (LogContext.PushProperty(ActivityIdHeader, activityId))
        using (LogContext.PushProperty(ActivityTraceIdHeader, activityTraceId))
        {
            await next(context);
        }
    }
}
