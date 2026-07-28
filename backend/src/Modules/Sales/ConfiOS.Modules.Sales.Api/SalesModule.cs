using ConfiOS.BuildingBlocks.Api.Modules;
using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Infrastructure.Interceptors;
using ConfiOS.Modules.Sales.Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfiOS.Modules.Sales.Api;

/// <summary>
/// Retail and wholesale sales, invoices and refunds.
/// </summary>
/// <remarks>
/// Scaffold only: the context and registration are in place so the module can be built out
/// against a working host, but no aggregates or endpoints exist yet. See the
/// implementation order in CLAUDE.md.
/// </remarks>
public sealed class SalesModule : IModule
{
    public string Name => SalesDbContext.SchemaName;

    public string? FeatureKey => "sales";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddDbContext<SalesDbContext>((provider, options) =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.MigrationsHistoryTable("__migrations", SalesDbContext.SchemaName));

            options.AddInterceptors(provider.GetRequiredService<AuditingInterceptor>());
        });
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // No endpoints yet.
    }
}
