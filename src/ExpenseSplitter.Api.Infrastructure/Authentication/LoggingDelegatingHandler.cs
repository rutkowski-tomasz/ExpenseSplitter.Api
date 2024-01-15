using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Api.Infrastructure.Authentication
{
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
                logger.LogTrace(
                    "Sending HTTP request to {Method} {Uri} with {Content}",
                    request.Method,
                    request.RequestUri,
                    request.Content
                );

                var result = await base.SendAsync(request, cancellationToken);

                logger.LogInformation("After HTTP request");

                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "HTTP request failed");

                throw;
            }
        }
    }
}
