using ExpenseSplitter.Application;
using ExpenseSplitter.Infrastructure;
using ExpenseSplitter.Presentation.Api.Endpoints;
using ExpenseSplitter.Presentation.Api.Extensions;

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
