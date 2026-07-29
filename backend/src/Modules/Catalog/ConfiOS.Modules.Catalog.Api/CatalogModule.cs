using ConfiOS.BuildingBlocks.Api.Modules;
using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Infrastructure.Interceptors;
using ConfiOS.BuildingBlocks.Infrastructure.Persistence;
using ConfiOS.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.Modules.Catalog.Api;

/// <summary>
/// Products, categories, brands, units and barcodes.
/// </summary>
/// <remarks>
/// Scaffold only: the context and registration are in place so the module can be built out
/// against a working host, but no aggregates or endpoints exist yet. See the
/// implementation order in CLAUDE.md.
/// </remarks>
public sealed class CatalogModule : IModule
{
    public string Name => CatalogDbContext.SchemaName;

    public string? FeatureKey => "catalog";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<CatalogDbContext>((provider, options) =>
        {
            options.UseConfiOsPostgres(
                configuration.GetConnectionString("Postgres"),
                CatalogDbContext.SchemaName);

            options.AddInterceptors(provider.GetRequiredService<AuditingInterceptor>());
        });
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // No endpoints yet.
    }
}
