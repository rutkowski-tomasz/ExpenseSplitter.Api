using ExpenseSplitter.Api.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Api.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ISender Sender;
    protected readonly ApplicationDbContext DbContext;
    private readonly IServiceScope scope;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory appFactory)
    {
        scope = appFactory.Services.CreateScope();

        Sender = scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public void Dispose()
    {
        scope.Dispose();
    }
}
