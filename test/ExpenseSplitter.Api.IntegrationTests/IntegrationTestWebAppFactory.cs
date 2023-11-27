using ExpenseSplitter.Api.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace ExpenseSplitter.Api.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("expensesplitter")
        .WithUsername("admin")
        .WithPassword("admin")
        .Build();

    public Task InitializeAsync()
    {
        return postgreSqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options
                    .UseNpgsql(postgreSqlContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention();
            });
        });
    }

    public new Task DisposeAsync()
    {
        return postgreSqlContainer.StopAsync();
    }
}
