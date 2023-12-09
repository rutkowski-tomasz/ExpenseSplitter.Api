using ExpenseSplitter.Api.Application;
using ExpenseSplitter.Api.Infrastructure;
using ExpenseSplitter.Api.Presentation.Expenses;
using ExpenseSplitter.Api.Presentation.Extensions;
using ExpenseSplitter.Api.Presentation.Settlements;
using ExpenseSplitter.Api.Presentation.User;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JSON Web Token based security",
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseTraceIdMiddleware();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app
    .MapSettlementEndpoints()
    .MapExpensesEndpoints()
    .MapUserEndpoints();

app.Run();

public partial class Program { }
