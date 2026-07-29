namespace ConfiOS.Modules.Identity.Api.Contracts;

/// <summary>
/// Sign-up payload.
/// </summary>
/// <remarks>
/// Kept separate from the command so the wire contract and the use case can evolve
/// independently. <c>OwnerPassword</c> is never logged: request body logging is off for
/// this route.
/// </remarks>
/// <param name="Name">Trading name of the business.</param>
/// <param name="Slug">URL-safe handle, unique across the platform.</param>
/// <param name="CountryCode">ISO 3166-1 alpha-2 country code.</param>
/// <param name="CurrencyCode">ISO 4217 currency code.</param>
/// <param name="DefaultLanguage">BCP 47 default language: en, rw or fr.</param>
/// <param name="TimeZoneId">IANA time zone.</param>
/// <param name="OwnerEmail">Email of the first owner account.</param>
/// <param name="OwnerFullName">Name of the first owner.</param>
/// <param name="OwnerPassword">Initial password.</param>
/// <param name="FirstBranchName">Name of the branch created with the business.</param>
public sealed record ProvisionTenantRequest(
    string Name,
    string Slug,
    string CountryCode,
    string CurrencyCode,
    string DefaultLanguage,
    string TimeZoneId,
    string OwnerEmail,
    string OwnerFullName,
    string OwnerPassword,
    string FirstBranchName);

/// <summary>Invitation payload.</summary>
/// <param name="Email">Email the invitation is sent to.</param>
/// <param name="FullName">Person's name.</param>
/// <param name="RoleIds">Roles granted on acceptance.</param>
/// <param name="BranchIds">Branches the user may work in.</param>
public sealed record InviteUserRequest(
    string Email,
    string FullName,
    IReadOnlyList<Guid> RoleIds,
    IReadOnlyList<Guid> BranchIds);

/// <summary>Sign-in payload.</summary>
/// <param name="BusinessHandle">The tenant's handle (slug) the user belongs to.</param>
/// <param name="Email">The user's email.</param>
/// <param name="Password">The user's password.</param>
public sealed record LoginRequest(string BusinessHandle, string Email, string Password);

/// <summary>The identity carried by the current access token.</summary>
/// <param name="UserId">Subject (user) identifier.</param>
/// <param name="TenantId">Tenant the token is scoped to.</param>
/// <param name="Email">The user's email.</param>
/// <param name="Permissions">Permission keys the token grants.</param>
public sealed record CurrentUserResponse(
    string? UserId,
    string? TenantId,
    string? Email,
    IReadOnlyList<string> Permissions);
