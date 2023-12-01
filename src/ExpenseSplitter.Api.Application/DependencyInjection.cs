using ExpenseSplitter.Api.Application.Abstractions.Behaviors;
using ExpenseSplitter.Api.Domain.ExpenseAllocations.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Api.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient<IExpenseAllocationService, ExpenseAllocationService>();

        return services;
    }
}
