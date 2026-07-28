using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain.Authorization;
using ConfiOS.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.Modules.Identity.Infrastructure.Repositories;

/// <summary>Role storage, scoped to the current tenant by the context's query filter.</summary>
/// <param name="context">Identity database context.</param>
public sealed class RoleRepository(IdentityDbContext context) : IRoleRepository
{
    public Task<Role?> GetAsync(Guid roleId, CancellationToken cancellationToken = default) =>
        context.Roles.FirstOrDefaultAsync(role => role.Id == roleId, cancellationToken);

    public Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
        context.Roles.FirstOrDefaultAsync(role => role.Name == name, cancellationToken);

    public async Task<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.Roles
            .OrderBy(role => role.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public void Add(Role role) => context.Roles.Add(role);
}
