using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Caching;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Allocations;
using ExpenseSplitter.Api.Domain.Expenses;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Users;
using ExpenseSplitter.Api.Infrastructure.Authentication;
using ExpenseSplitter.Api.Infrastructure.Caching;
using ExpenseSplitter.Api.Infrastructure.Repositories;
using ExpenseSplitter.Api.Infrastructure.Settlements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ExpenseSplitter.Api.Infrastructure.Idempotency;
using ExpenseSplitter.Api.Application.Abstractions.Idempotency;
using Asp.Versioning;
using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Abstractions.Etag;
using ExpenseSplitter.Api.Infrastructure.Clock;
using ExpenseSplitter.Api.Infrastructure.Etag;
using ExpenseSplitter.Api.Infrastructure.Serializer;
using Microsoft.Extensions.Caching.Hybrid;

namespace ExpenseSplitter.Api.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services, configuration);

        AddAuthentication(services, configuration);

        AddCaching(services);
        
        AddSerializer(services);

        AddVersioning(services);

        services.AddScoped<IIdempotencyService, IdempotencyService>();
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
                httpClient.BaseAddress = new Uri(keycloakOptions.BaseUrl);
            })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>()
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .AddStandardResilienceHandler();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
            {
                var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                httpClient.BaseAddress = new Uri(keycloakOptions.BaseUrl);
            })
            .AddHttpMessageHandler<LoggingDelegatingHandler>()
            .AddStandardResilienceHandler();

        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IInviteCodeService, InviteCodeService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IEtagService, EtagService>();
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Database") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
        );

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

    private static void AddCaching(IServiceCollection services)
    {
        #pragma warning disable EXTEXP0018
        services.AddHybridCache(options =>
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(5),
                Expiration = TimeSpan.FromMinutes(5)
            }
        );
        #pragma warning restore EXTEXP0018

        #pragma warning disable S125
        // services.AddStackExchangeRedisCache(redisOptions =>
        // {
        //     var connection = configuration.GetConnectionString("Redis");
        //
        //     redisOptions.Configuration = connection;
        // });
        // services.AddTransient<ICacheService, DistributedCacheService>();

        // services.AddSingleton<ICacheService, InMemoryCacheService>();
        // services.AddMemoryCache();
        #pragma warning restore S125
    }

    private static void AddSerializer(IServiceCollection services)
    {
        services.AddTransient<ISerializer, MessagePackSerializer>();
    }

    private static void AddVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }
}
