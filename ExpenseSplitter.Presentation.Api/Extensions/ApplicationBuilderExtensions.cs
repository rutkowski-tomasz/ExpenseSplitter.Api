using ExpenseSplitter.Infrastructure;
using ExpenseSplitter.Presentation.Api.Middleware;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Presentation.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }

    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void UseTraceIdMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<TraceIdMiddleware>();
    }
}
