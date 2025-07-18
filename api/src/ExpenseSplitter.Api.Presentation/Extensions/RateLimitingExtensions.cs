using System.Globalization;
using System.Threading.RateLimiting;

namespace ExpenseSplitter.Api.Presentation.Extensions;

public static class RateLimitingExtensions
{
    public const string UserRateLimiting = "fixed-by-user";
    public const string IpRateLimiting = "fixed-by-ip";

    public static void AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options => {
            options.AddPolicy(UserRateLimiting, httpContext => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name?.ToString(),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(5)
                }
            ));

            options.AddPolicy(IpRateLimiting, httpContext => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(5)
                }
            ));

            options.OnRejected = (context, cancellationToken) => {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int) retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);

                return new ValueTask();
            };
        });
    }
}
