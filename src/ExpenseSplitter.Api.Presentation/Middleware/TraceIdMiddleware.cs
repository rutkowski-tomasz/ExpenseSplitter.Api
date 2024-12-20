using Microsoft.AspNetCore.Http.Features;
using Serilog.Context;

namespace ExpenseSplitter.Api.Presentation.Middleware;

public class TraceIdMiddleware(RequestDelegate next)
{
    private const string TraceIdHeader = "traceId";
    private const string RequestIdHeader = "requestId";

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.TraceIdentifier;
        context.Response.Headers.TryAdd(RequestIdHeader, requestId);

        var traceId = context.Features.Get<IHttpActivityFeature>()?.Activity.Id;
        context.Response.Headers.TryAdd(TraceIdHeader, traceId);
        
        using (LogContext.PushProperty("requestId", requestId))
        using (LogContext.PushProperty("traceId", traceId))
        {
            await next(context);
        }
    }
}
