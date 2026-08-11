namespace ConfiOS.Modules.Identity.Application.Abstractions;

/// <summary>
/// Issues and validates the short-lived token that carries a completed password
/// check between the two halves of a multi-business sign-in.
/// </summary>
/// <remarks>
/// The token exists so the password is verified exactly once. It names the
/// businesses the credentials already unlocked, so the second call cannot be
/// steered towards a business the caller never proved access to, and it is not an
/// access token: it grants nothing on its own.
/// </remarks>
public interface IBusinessSelectionTokens
{
    BusinessSelectionToken Issue(string email, IReadOnlyCollection<Guid> tenantIds);

    /// <summary>Returns null when the token is missing, expired, altered or of the wrong kind.</summary>
    BusinessSelectionPayload? Validate(string token);
}

/// <param name="Value">The encoded token.</param>
/// <param name="ExpiresAt">When it stops being accepted.</param>
public sealed record BusinessSelectionToken(string Value, DateTimeOffset ExpiresAt);

/// <param name="Email">The email whose password was verified.</param>
/// <param name="TenantIds">Businesses those credentials unlocked.</param>
public sealed record BusinessSelectionPayload(string Email, IReadOnlyList<Guid> TenantIds);
