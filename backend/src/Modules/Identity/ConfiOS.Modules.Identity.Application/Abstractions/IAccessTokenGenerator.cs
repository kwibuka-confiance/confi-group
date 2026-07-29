using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.Modules.Identity.Application.Abstractions;

/// <summary>Issues signed access tokens for authenticated users.</summary>
public interface IAccessTokenGenerator
{
    AccessToken Generate(AccessTokenClaims claims);
}

/// <summary>A signed access token and the moment it expires.</summary>
/// <param name="Value">The encoded JWT.</param>
/// <param name="ExpiresAt">When the token stops being valid.</param>
public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);

/// <summary>Everything that goes into a user's access token.</summary>
/// <param name="TenantId">Tenant the session is for.</param>
/// <param name="UserId">The signed-in user.</param>
/// <param name="Email">The user's email.</param>
/// <param name="CurrencyCode">Tenant's default ISO 4217 currency.</param>
/// <param name="TimeZoneId">Tenant's IANA time zone.</param>
/// <param name="Permissions">Permission keys granted to the user.</param>
public sealed record AccessTokenClaims(
    TenantId TenantId,
    UserId UserId,
    string Email,
    string CurrencyCode,
    string TimeZoneId,
    IReadOnlyCollection<string> Permissions);
