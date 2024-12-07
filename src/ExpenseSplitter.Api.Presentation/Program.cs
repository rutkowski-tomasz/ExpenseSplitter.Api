using ExpenseSplitter.Api.Application;
using ExpenseSplitter.Api.Infrastructure;
using ExpenseSplitter.Api.Presentation.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpoints(typeof(Program).Assembly);
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddUrlGroup(new Uri(builder.Configuration["Keycloak:BaseUrl"]!), HttpMethod.Get, "keycloak")
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

builder.Services.AddRateLimiting();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint(
        "/openapi/v1/swagger.json",
        "ExpenseSplitter.Api v1"
    ));

    app.ApplyMigrations();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseTraceIdMiddleware();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.UseEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public partial class Program;
