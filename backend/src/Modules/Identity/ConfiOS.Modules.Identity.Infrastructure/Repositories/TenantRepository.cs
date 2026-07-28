using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain.Tenants;
using ConfiOS.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.Modules.Identity.Infrastructure.Repositories;

/// <summary>
/// Tenant storage. Operates at the platform level: a tenant is not itself tenant-scoped,
/// so no query filter applies and lookups are by primary key or slug only.
/// </summary>
/// <param name="context">Identity database context.</param>
public sealed class TenantRepository(IdentityDbContext context) : ITenantRepository
{
    public Task<Tenant?> GetAsync(TenantId tenantId, CancellationToken cancellationToken = default) =>
        context.Tenants
            .Include(tenant => tenant.Branches)
            .FirstOrDefaultAsync(tenant => tenant.Id == tenantId.Value, cancellationToken);

    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        context.Tenants.FirstOrDefaultAsync(tenant => tenant.Slug == slug, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default) =>
        context.Tenants.AnyAsync(tenant => tenant.Slug == slug, cancellationToken);

    public void Add(Tenant tenant) => context.Tenants.Add(tenant);
}
