using ExpenseSplitter.Api.Application.Abstractions.Behaviors;
using ExpenseSplitter.Api.Application.Abstractions.Caching;
using ExpenseSplitter.Api.Application.Abstractions.Idempotency;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseSplitter.Api.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(IdempotentBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
    }
}
