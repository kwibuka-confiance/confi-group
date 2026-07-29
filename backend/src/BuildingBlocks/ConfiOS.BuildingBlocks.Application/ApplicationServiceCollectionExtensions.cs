using ConfiOS.BuildingBlocks.Application.Authorization;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ConfiOS.BuildingBlocks.Application;

/// <summary>Registers the shared application services.</summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Adds the dispatcher and the per-request tenant context. Call once from the host;
    /// modules register their own handlers and validators.
    /// </summary>
    public static IServiceCollection AddBuildingBlocksApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<IDispatcher, Dispatcher>();
        services.TryAddScoped<AmbientContext>();
        services.TryAddScoped<ITenantContext>(provider => provider.GetRequiredService<AmbientContext>());
        services.TryAddSingleton<IPermissionRegistry, PermissionRegistry>();

        return services;
    }
}
