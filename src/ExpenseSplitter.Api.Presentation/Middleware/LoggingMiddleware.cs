using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.IO;

namespace ExpenseSplitter.Api.Presentation.Middleware;

public class LoggingMiddleware
{
    private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager = new();
    private readonly ILogger<LoggingMiddleware> logger;
    private readonly RequestDelegate next;
    private readonly IWebHostEnvironment environment;

    public LoggingMiddleware(
        ILogger<LoggingMiddleware> logger,
        RequestDelegate next,
        IWebHostEnvironment environment
    )
    {
        this.logger = logger;
        this.next = next;
        this.environment = environment;
    }

    public async Task Invoke(HttpContext context)
    {
        await using var originalBodyStream = context.Response.Body;

        try
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                await LogRequest(context);
            }

            await using var responseBody = recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await next(context);

            if (logger.IsEnabled(GetResponseLogLevel(context)))
            {
                await LogResponse(context, responseBody, originalBodyStream);
            }
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        logger.LogInformation(
            "Request {Method} {Url} Body:{Body} Headers:{@Headers}",
            context.Request.Method,
            context.Request.GetDisplayUrl(),
            await GetRequestBodyString(context.Request),
            GetHeadersDictionary(context.Request.Headers)
        );
    }

    private static async Task<string> GetRequestBodyString(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return body;
    }

    private static Dictionary<string, string> GetHeadersDictionary(IHeaderDictionary headers)
    {
        return headers.ToDictionary(x => x.Key, x => x.Value.ToString());
    }

    private static LogLevel GetResponseLogLevel(HttpContext context)
    {
        return context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Debug;
    }

    private async Task LogResponse(HttpContext context, Stream responseBody, Stream originalBodyStream)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);

        logger.Log(
            GetResponseLogLevel(context),
            "Response {StatusCode} Body:{Body} Headers:{@Headers}",
            context.Response.StatusCode,
            body,
            GetHeadersDictionary(context.Response.Headers)
        );
    }
}