using ConfiOS.BuildingBlocks.Domain.Events;

namespace ConfiOS.Modules.Identity.Domain.Events;

/// <summary>Raised when a business is provisioned. Other modules seed their defaults from this.</summary>
/// <param name="TenantId">The new tenant.</param>
/// <param name="Name">Trading name.</param>
/// <param name="CountryCode">ISO 3166-1 alpha-2 country.</param>
/// <param name="CurrencyCode">ISO 4217 default currency.</param>
/// <param name="DefaultLanguage">BCP 47 default language.</param>
public sealed record TenantCreated(
    Guid TenantId,
    string Name,
    string CountryCode,
    string CurrencyCode,
    string DefaultLanguage) : DomainEvent("identity.tenant-created");

/// <summary>Raised when a tenant is suspended, for example for non-payment.</summary>
/// <param name="TenantId">The suspended tenant.</param>
/// <param name="Reason">Why it was suspended.</param>
public sealed record TenantSuspended(Guid TenantId, string Reason) : DomainEvent("identity.tenant-suspended");

/// <summary>Raised when a branch is opened. Inventory creates a default warehouse in response.</summary>
/// <param name="TenantId">Owning tenant.</param>
/// <param name="BranchId">The new branch.</param>
/// <param name="Name">Branch name.</param>
public sealed record BranchCreated(Guid TenantId, Guid BranchId, string Name)
    : DomainEvent("identity.branch-created");

/// <summary>Raised when a user is added to a tenant.</summary>
/// <param name="TenantId">Owning tenant.</param>
/// <param name="UserId">The new user.</param>
/// <param name="Email">Email the invitation was sent to.</param>
public sealed record UserInvited(Guid TenantId, Guid UserId, string Email)
    : DomainEvent("identity.user-invited");

/// <summary>Raised when a user is deactivated. Sessions are revoked in response.</summary>
/// <param name="TenantId">Owning tenant.</param>
/// <param name="UserId">The deactivated user.</param>
public sealed record UserDeactivated(Guid TenantId, Guid UserId) : DomainEvent("identity.user-deactivated");

/// <summary>Raised when a user changes their language. Notifications switch template language.</summary>
/// <param name="TenantId">Owning tenant.</param>
/// <param name="UserId">The user.</param>
/// <param name="Language">New BCP 47 language tag.</param>
public sealed record LanguageChanged(Guid TenantId, Guid UserId, string Language)
    : DomainEvent("identity.language-changed");
