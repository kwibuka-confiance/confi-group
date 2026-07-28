using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.Modules.Identity.Domain.Authorization;

/// <summary>
/// A named set of permissions within one tenant.
/// </summary>
/// <remarks>
/// System roles are seeded for every tenant and cannot be renamed or deleted, because
/// authorization checks elsewhere assume they exist. Tenants may create their own roles
/// freely.
/// </remarks>
public sealed class Role : TenantEntity
{
    private readonly List<RolePermission> _permissions = [];

    private Role(Guid id, TenantId tenantId, string name, bool isSystemRole)
        : base(id, tenantId)
    {
        Name = name;
        IsSystemRole = isSystemRole;
    }

    private Role()
    {
    }

    /// <summary>Stable key unique within the tenant, for example <c>branch-manager</c>.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Seeded with the tenant and protected from renaming or deletion.</summary>
    public bool IsSystemRole { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    public static Role Create(TenantId tenantId, string name, bool isSystemRole = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Role(Guid.CreateVersion7(), tenantId, name.Trim().ToLowerInvariant(), isSystemRole);
    }

    public void Grant(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);

        if (_permissions.Exists(item => string.Equals(item.Permission, permission, StringComparison.Ordinal)))
        {
            return;
        }

        _permissions.Add(new RolePermission(Id, permission));
    }

    public void GrantAll(IEnumerable<string> permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);

        foreach (var permission in permissions)
        {
            Grant(permission);
        }
    }

    public void Revoke(string permission)
    {
        EnsureMutable();
        _permissions.RemoveAll(item => string.Equals(item.Permission, permission, StringComparison.Ordinal));
    }

    public void Rename(string name)
    {
        EnsureMutable();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim().ToLowerInvariant();
    }

    public bool Has(string permission) =>
        _permissions.Exists(item => string.Equals(item.Permission, permission, StringComparison.Ordinal));

    private void EnsureMutable()
    {
        if (IsSystemRole)
        {
            throw new DomainException(Error.Forbidden(IdentityErrorCodes.SystemRoleImmutable));
        }
    }
}

/// <summary>A permission granted to a role.</summary>
/// <param name="RoleId">The role.</param>
/// <param name="Permission">Permission key, for example <c>identity.users.invite</c>.</param>
public sealed record RolePermission(Guid RoleId, string Permission);
