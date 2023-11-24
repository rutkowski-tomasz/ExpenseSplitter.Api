using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Expenses;
using ExpenseSplitter.Domain.Settlements;
using ExpenseSplitter.Domain.Users;
using ExpenseSplitter.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>())
            .AddScoped<IExpenseRepository, ExpenseRepository>()
            .AddScoped<ISettlementRepository, SettlementRepository>()
            .AddScoped<IUserRepository, UserRepository>()
        ;
    }
}