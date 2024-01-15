using ExpenseSplitter.Api.Application.Abstraction.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Users;
using ExpenseSplitter.Api.Infrastructure.Authentication;
using ExpenseSplitter.Api.Infrastructure.Configurations;
using ExpenseSplitter.Api.Infrastructure.Repositories;
using ExpenseSplitter.Api.Infrastructure.Settlements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ExpenseSplitter.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);

        AddAuthentication(services, configuration);

        return services;
    }
    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

        services.ConfigureOptions<JwtBearerOptionsSetup>();
        
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.AddTransient<AdminAuthorizationDelegatingHandler>();
        services.AddTransient<LoggingDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
            {
                var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
            })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>()
            .AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
            {
                var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
            })
            .AddHttpMessageHandler<LoggingDelegatingHandler>();

        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IInviteCodeService, InviteCodeService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
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
            .AddScoped<IAllocationRepository, AllocationRepository>()
            .AddScoped<IParticipantRepository, ParticipantRepository>()
            .AddScoped<ISettlementUserRepository, SettlementUserRepository>()
        ;
    }
}