using ExpenseSplitter.Api.Presentation.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace ExpenseSplitter.Api.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        var serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false }
                && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        services.AddTransient(typeof(IHandler<,,,>), typeof(Handler<,,,>));
        services.AddTransient(typeof(IHandlerEmptyRequest<,,>), typeof(HandlerEmptyRequest<,,>));
        services.AddTransient(typeof(IHandlerEmptyResponse<,>), typeof(HandlerEmptyResponse<,>));

        AddMappers(services, assembly);

        return services;
    }

    private static void AddMappers(IServiceCollection services, Assembly assembly)
    {
        var mapperTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => 
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IMapper<,>)))
            .ToList();

        foreach (var type in mapperTypes)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapper<,>));

            foreach (var intf in interfaces)
            {
                services.AddTransient(intf, type);
            }
        }
    }
}

