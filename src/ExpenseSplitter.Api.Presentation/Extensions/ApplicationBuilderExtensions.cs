using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Presentation.Middleware;
using ExpenseSplitter.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ExpenseSplitter.Api.Presentation.Abstractions;
using Asp.Versioning;

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

    public static void UseEndpoints(this IApplicationBuilder app)
    {
        var webApplication = (WebApplication) app;

        var apiVersionSet = webApplication
            .NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var versionedRouteGroupBuilder = webApplication
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        var endpoints = webApplication.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(versionedRouteGroupBuilder);
        }
    }
}
