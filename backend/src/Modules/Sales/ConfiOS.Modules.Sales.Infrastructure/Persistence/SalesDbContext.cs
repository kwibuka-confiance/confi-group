using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.Modules.Sales.Infrastructure.Persistence;

/// <summary>
/// Persistence for the Sales module. Owns the <c>sales</c> schema exclusively; no other
/// module reads or writes these tables.
/// </summary>
/// <param name="options">EF Core options supplied by the host.</param>
/// <param name="tenantContext">Tenant resolved for the current request.</param>
/// <param name="clock">Source of the current time.</param>
public sealed class SalesDbContext(
    DbContextOptions<SalesDbContext> options,
    ITenantContext tenantContext,
    IClock clock) : TenantDbContext(options, tenantContext, clock)
{
    /// <summary>Schema name, also the module name.</summary>
    public const string SchemaName = "sales";

    protected override string Schema => SchemaName;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
