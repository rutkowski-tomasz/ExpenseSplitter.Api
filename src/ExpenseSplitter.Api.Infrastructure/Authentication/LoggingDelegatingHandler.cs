using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Infrastructure.Authentication;

public class LoggingDelegatingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingDelegatingHandler> logger;

    public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
    {
        this.logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var requestContent = await request.Content!.ReadAsStringAsync();
            logger.LogInformation(
                "Sending HTTP request to {Method} {Uri} with {Content}",
                request.Method.ToString(),
                request.RequestUri!.ToString(),
                requestContent
            );

            var stopWatch = Stopwatch.StartNew();
            var result = await base.SendAsync(request, cancellationToken);

            var resultContent = await result.Content.ReadAsStringAsync();
            logger.LogInformation(
                "Received HTTP response {StatusCode} in {Duration}ms with {Content}",
                result.StatusCode,
                stopWatch.ElapsedMilliseconds,
                resultContent
            );

            return result;
        }
        catch (Exception e)
        {
            logger.LogError(e, "HTTP request failed");

            throw;
        }
    }
}
