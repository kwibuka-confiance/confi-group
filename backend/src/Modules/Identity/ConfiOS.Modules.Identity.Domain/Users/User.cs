using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Domain.Events;
using ConfiOS.Modules.Identity.Domain.Tenants;

namespace ConfiOS.Modules.Identity.Domain.Users;

/// <summary>
/// A person who signs in to a tenant.
/// </summary>
/// <remarks>
/// A user belongs to one tenant. Somebody who works for two businesses has two user
/// records, which keeps MT-001 intact and means revoking access at one business cannot
/// affect the other.
/// </remarks>
public sealed class User : TenantEntity
{
    private readonly List<UserRole> _roles = [];
    private readonly List<UserBranch> _branches = [];

    private User(Guid id, TenantId tenantId, EmailAddress email, string fullName, string passwordHash)
        : base(id, tenantId)
    {
        Email = email;
        FullName = fullName;
        PasswordHash = passwordHash;
        Status = UserStatus.Invited;
    }

    private User()
    {
    }

    public EmailAddress Email { get; private set; } = null!;

    public string FullName { get; private set; } = string.Empty;

    public PhoneNumber? Phone { get; private set; }

    /// <summary>
    /// Salted hash produced by the infrastructure layer. The plain password is never held
    /// on this type and never logged.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    public UserStatus Status { get; private set; }

    /// <summary>Overrides the tenant default when set (MT-007).</summary>
    public string? PreferredLanguage { get; private set; }

    public DateTimeOffset? LastSignedInAt { get; private set; }

    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    public IReadOnlyCollection<UserBranch> Branches => _branches.AsReadOnly();

    public static User Invite(TenantId tenantId, EmailAddress email, string fullName, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        var user = new User(Guid.CreateVersion7(), tenantId, email, fullName.Trim(), passwordHash);
        user.Raise(new UserInvited(tenantId.Value, user.Id, email.Value));

        return user;
    }

    public void AssignRole(Guid roleId)
    {
        if (_roles.Exists(role => role.RoleId == roleId))
        {
            return;
        }

        _roles.Add(new UserRole(Id, roleId));
    }

    public void RemoveRole(Guid roleId) => _roles.RemoveAll(role => role.RoleId == roleId);

    /// <summary>Grants access to a branch. The branch must belong to this user's tenant (MT-005).</summary>
    public void GrantBranchAccess(BranchId branchId)
    {
        if (_branches.Exists(branch => branch.BranchId == branchId.Value))
        {
            return;
        }

        _branches.Add(new UserBranch(Id, branchId.Value));
    }

    public void RevokeBranchAccess(BranchId branchId) =>
        _branches.RemoveAll(branch => branch.BranchId == branchId.Value);

    /// <summary>Completes an invitation once the person sets their password.</summary>
    public void Activate(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        PasswordHash = passwordHash;
        Status = UserStatus.Active;
    }

    public void ChangePassword(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        EnsureActive();

        PasswordHash = passwordHash;
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Deactivated)
        {
            return;
        }

        Status = UserStatus.Deactivated;
        Raise(new UserDeactivated(TenantId, Id));
    }

    public void Reactivate() => Status = UserStatus.Active;

    public void ChangeLanguage(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        var normalised = language.Trim().ToLowerInvariant();

        if (!TenantSettings.SupportedLanguages.Contains(normalised, StringComparer.Ordinal))
        {
            throw new DomainException(Error.Validation(IdentityErrorCodes.UnsupportedLanguage));
        }

        PreferredLanguage = normalised;
        Raise(new LanguageChanged(TenantId, Id, normalised));
    }

    public void RecordSignIn(DateTimeOffset at)
    {
        EnsureActive();
        LastSignedInAt = at;
    }

    /// <summary>
    /// A deactivated user must not be able to sign in or act, so every state-changing
    /// path goes through this check.
    /// </summary>
    private void EnsureActive()
    {
        if (Status != UserStatus.Active)
        {
            throw new DomainException(Error.Forbidden(IdentityErrorCodes.UserDeactivated));
        }
    }
}

/// <summary>Lifecycle state of a user.</summary>
public enum UserStatus
{
    /// <summary>Invited but has not yet set a password.</summary>
    Invited = 0,
    Active = 1,
    Deactivated = 2,
}

/// <summary>Join record between a user and a role.</summary>
/// <param name="UserId">The user.</param>
/// <param name="RoleId">The role granted.</param>
public sealed record UserRole(Guid UserId, Guid RoleId);

/// <summary>Join record between a user and a branch they may work in (MT-004).</summary>
/// <param name="UserId">The user.</param>
/// <param name="BranchId">The branch.</param>
public sealed record UserBranch(Guid UserId, Guid BranchId);
