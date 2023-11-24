using ExpenseSplitter.Domain.Expenses;
using ExpenseSplitter.Domain.Settlements;
using ExpenseSplitter.Domain.Users;
using ExpenseSplitter.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        AddPersistence(services);

        return services;
    }

    private static void AddPersistence(IServiceCollection services)
    {
        services
            .AddScoped<IExpenseRepository, ExpenseRepository>()
            .AddScoped<ISettlementRepository, SettlementRepository>()
            .AddScoped<IUserRepository, UserRepository>()
        ;
    }
}