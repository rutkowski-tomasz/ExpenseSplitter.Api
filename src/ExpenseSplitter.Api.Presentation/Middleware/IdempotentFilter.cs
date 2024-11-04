using ExpenseSplitter.Api.Application.Abstractions.Idempotency;

namespace ExpenseSplitter.Api.Presentation.Middleware;

// Note there is also IdempotentFilter which works on command, query level
internal sealed class IdempotentFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var idempotencyService = context.HttpContext.RequestServices.GetRequiredService<IIdempotencyService>();

        var idempotencyKeyResult = idempotencyService.GetIdempotencyKeyFromHeaders();
        if (idempotencyKeyResult.IsFailure)
        {
            return await next(context);
        }

        var idempotencyKey = idempotencyKeyResult.Value;
        var processedRequestResult = await idempotencyService.GetProcessedRequest<ProcessedIdempotentRequest>(
            idempotencyKey,
            context.HttpContext.RequestAborted
        );

        if (processedRequestResult.isProcessed)
        {
            var processedRequest = processedRequestResult.result!;
            context.HttpContext.Response.Headers.Append("X-Cached-Response", idempotencyKey.ToString());
            return Results.Json(processedRequest.Response, statusCode: processedRequest.StatusCode);
        }
        
        var result = await next(context);

        if (result is not (IStatusCodeHttpResult { StatusCode: >= 200 and < 300 } statusCodeResult
            and IValueHttpResult valueResult))
        {
            return result;
        }

        var idempotentRequest = new ProcessedIdempotentRequest(statusCodeResult.StatusCode.Value, valueResult.Value);

        await idempotencyService.SaveIdempotentRequest(
            idempotencyKey,
            idempotentRequest,
            context.HttpContext.RequestAborted
        );

        return result;
    }
}
