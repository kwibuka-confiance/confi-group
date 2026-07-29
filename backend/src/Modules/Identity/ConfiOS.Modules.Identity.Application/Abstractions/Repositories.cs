using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Identity.Domain.Authorization;
using ConfiOS.Modules.Identity.Domain.Tenants;
using ConfiOS.Modules.Identity.Domain.Users;

namespace ConfiOS.Modules.Identity.Application.Abstractions;

/// <summary>Loads and stores tenants. Operates at the platform level, above tenant filtering.</summary>
public interface ITenantRepository
{
    Task<Tenant?> GetAsync(TenantId tenantId, CancellationToken cancellationToken = default);

    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);

    void Add(Tenant tenant);
}

/// <summary>Loads and stores users within the resolved tenant.</summary>
public interface IUserRepository
{
    Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a user for authentication by tenant and email, bypassing the tenant query
    /// filter because sign-in runs before any tenant context has been resolved.
    /// </summary>
    Task<User?> GetForAuthenticationAsync(
        TenantId tenantId,
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    void Add(User user);
}

/// <summary>Loads and stores roles within the resolved tenant.</summary>
public interface IRoleRepository
{
    Task<Role?> GetAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default);

    void Add(Role role);
}
