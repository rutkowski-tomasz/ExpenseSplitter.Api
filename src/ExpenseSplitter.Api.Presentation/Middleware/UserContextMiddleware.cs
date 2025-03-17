using System.Diagnostics;
using System.Security.Claims;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using Serilog.Context;

namespace ExpenseSplitter.Api.Presentation.Middleware;

public class UserContextMiddleware(RequestDelegate next)
{
    private const string UserIdPropertyName = "UserId";

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            await next(context);
            return;
        }

        Activity.Current?.SetTag(UserIdPropertyName, userId);
        using (LogContext.PushProperty(UserIdPropertyName, userId))
        {
            await next(context);
        }
    }
}
