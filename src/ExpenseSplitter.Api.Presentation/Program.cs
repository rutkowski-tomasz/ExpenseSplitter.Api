using ExpenseSplitter.Api.Application;
using ExpenseSplitter.Api.Infrastructure;
using ExpenseSplitter.Api.Presentation.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using DotNetEnv;
using OpenTelemetry.Logs;

Env.Load("../../.env");

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddSerilog(config => config
    .ReadFrom.Configuration(builder.Configuration), writeToProviders: true
);

builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());

builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddUrlGroup(new Uri(builder.Configuration["Keycloak:BaseUrl"]!), HttpMethod.Get, "keycloak")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

builder.Services.AddRateLimiting();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint(
        "/openapi/v1.json",
        "ExpenseSplitter.Api v1"
    ));

    app.ApplyMigrations();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseTraceIdMiddleware();

app.UseUserContextMiddleware();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.UseEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

await app.RunAsync();

public partial class Program;
