using ExpenseSplitter.Api.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Api.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope scope;
    protected readonly ISender Sender;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory appFactory)
    {
        scope = appFactory.Services.CreateScope();

        Sender = scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
}
