using ConfiOS.BuildingBlocks.Api.Modules;
using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Infrastructure.Interceptors;
using ConfiOS.BuildingBlocks.Infrastructure.Persistence;
using ConfiOS.Modules.Inventory.Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.Modules.Inventory.Api;

/// <summary>
/// Warehouses, stock balances and stock movements.
/// </summary>
/// <remarks>
/// Scaffold only: the context and registration are in place so the module can be built out
/// against a working host, but no aggregates or endpoints exist yet. See the
/// implementation order in CLAUDE.md.
/// </remarks>
public sealed class InventoryModule : IModule
{
    public string Name => InventoryDbContext.SchemaName;

    public string? FeatureKey => "inventory";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<InventoryDbContext>((provider, options) =>
        {
            options.UseConfiOsPostgres(
                configuration.GetConnectionString("Postgres"),
                InventoryDbContext.SchemaName);

            options.AddInterceptors(provider.GetRequiredService<AuditingInterceptor>());
        });
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // No endpoints yet.
    }
}
