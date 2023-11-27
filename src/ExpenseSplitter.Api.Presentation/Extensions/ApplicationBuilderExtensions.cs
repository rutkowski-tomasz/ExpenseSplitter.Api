using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Presentation.Middleware;
using ExpenseSplitter.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Presentation.Extensions;

[ExcludeFromCodeCoverage]
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
