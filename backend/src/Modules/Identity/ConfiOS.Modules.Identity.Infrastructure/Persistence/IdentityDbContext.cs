using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Infrastructure.Persistence;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain.Authorization;
using ConfiOS.Modules.Identity.Domain.Tenants;
using ConfiOS.Modules.Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.Modules.Identity.Infrastructure.Persistence;

/// <summary>
/// Persistence for the Identity module. Owns the <c>identity</c> schema and nothing else:
/// no other module's context maps these tables, and this one maps no other module's
/// (docs/03-architecture/03-modular-monolith.md).
/// </summary>
/// <param name="options">EF Core options supplied by the host.</param>
/// <param name="tenantContext">Tenant resolved for the current request.</param>
/// <param name="clock">Source of the current time.</param>
public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    ITenantContext tenantContext,
    IClock clock) : TenantDbContext(options, tenantContext, clock), IIdentityUnitOfWork
{
    /// <summary>Schema name, also the module name.</summary>
    public const string SchemaName = "identity";

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    protected override string Schema => SchemaName;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
