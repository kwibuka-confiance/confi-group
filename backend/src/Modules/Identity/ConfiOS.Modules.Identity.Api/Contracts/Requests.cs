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
/// <param name="Email">The user's email.</param>
/// <param name="Password">The user's password.</param>
/// <param name="BusinessHandle">
/// Optional. Supplying it signs straight in to that business; leaving it out lets
/// the caller choose when the credentials unlock more than one.
/// </param>
public sealed record LoginRequest(string Email, string Password, string? BusinessHandle = null);

/// <summary>Completes a sign-in that offered several businesses.</summary>
/// <param name="SelectionToken">Token returned by the sign-in call.</param>
/// <param name="TenantId">The chosen business.</param>
public sealed record SelectBusinessRequest(string SelectionToken, Guid TenantId);

/// <summary>
/// Sign-in response. <c>status</c> is <c>authenticated</c> when a session was
/// issued and <c>select_business</c> when the caller must choose one first.
/// </summary>
/// <param name="Status">Which of the two shapes this is.</param>
/// <param name="AccessToken">Session token, when authenticated.</param>
/// <param name="ExpiresAt">Expiry of whichever token is present.</param>
/// <param name="UserId">The signed-in user, when authenticated.</param>
/// <param name="TenantId">The business, when authenticated.</param>
/// <param name="BusinessName">Trading name, when authenticated.</param>
/// <param name="FullName">The user's name, when authenticated.</param>
/// <param name="Email">The user's email, when authenticated.</param>
/// <param name="Permissions">Granted permissions, when authenticated.</param>
/// <param name="SelectionToken">Short-lived token to send back with the choice.</param>
/// <param name="Businesses">The businesses to choose between.</param>
public sealed record SignInResponse(
    string Status,
    string? AccessToken = null,
    DateTimeOffset? ExpiresAt = null,
    Guid? UserId = null,
    Guid? TenantId = null,
    string? BusinessName = null,
    string? FullName = null,
    string? Email = null,
    IReadOnlyList<string>? Permissions = null,
    string? SelectionToken = null,
    IReadOnlyList<BusinessSummary>? Businesses = null);

/// <summary>A business offered during sign-in.</summary>
/// <param name="TenantId">The business identifier.</param>
/// <param name="Name">Trading name.</param>
/// <param name="Slug">URL-safe handle.</param>
public sealed record BusinessSummary(Guid TenantId, string Name, string Slug);

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
