using Asp.Versioning;
using ExpenseSplitter.Api.Application;
using ExpenseSplitter.Api.Infrastructure;
using ExpenseSplitter.Api.Presentation.Expenses;
using ExpenseSplitter.Api.Presentation.Extensions;
using ExpenseSplitter.Api.Presentation.Settlements;
using ExpenseSplitter.Api.Presentation.User;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JSON Web Token based security",
    })
);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddUrlGroup(new Uri(builder.Configuration["Keycloak:BaseUrl"]!), HttpMethod.Get, "keycloak");

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseTraceIdMiddleware();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

var apiVersionSet = app
    .NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var routeGroupBuilder = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

routeGroupBuilder
    .MapSettlementEndpoints()
    .MapExpensesEndpoints()
    .MapUserEndpoints();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

public partial class Program { }
