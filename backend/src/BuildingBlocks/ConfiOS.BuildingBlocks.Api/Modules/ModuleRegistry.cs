using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.BuildingBlocks.Api.Modules;

/// <summary>
/// Discovers and wires up the modules the host was compiled with.
/// </summary>
/// <remarks>
/// Discovered modules are registered in the container rather than held in a static list,
/// so two hosts in one process (which is what integration tests are) do not see each
/// other's modules.
/// </remarks>
public static class ModuleRegistry
{
    /// <summary>
    /// Instantiates every <see cref="IModule"/> in the supplied assemblies, lets each
    /// register its services, and registers the module itself for endpoint mapping.
    /// </summary>
    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        var moduleTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IModule).IsAssignableFrom(type)
                && type is { IsAbstract: false, IsInterface: false })
            .OrderBy(type => type.Name, StringComparer.Ordinal);

        foreach (var type in moduleTypes)
        {
            if (Activator.CreateInstance(type) is not IModule module)
            {
                continue;
            }

            module.RegisterServices(services, configuration);
            services.AddSingleton(module);
        }

        return services;
    }

    /// <summary>Maps every registered module's endpoints.</summary>
    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        foreach (var module in endpoints.ServiceProvider.GetServices<IModule>())
        {
            module.MapEndpoints(endpoints);
        }

        return endpoints;
    }
}
