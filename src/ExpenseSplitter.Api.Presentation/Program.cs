using ExpenseSplitter.Api.Application;
using ExpenseSplitter.Api.Infrastructure;
using ExpenseSplitter.Api.Presentation.Endpoints;
using ExpenseSplitter.Api.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.MapSettlementEndpoints();

app.Run();
