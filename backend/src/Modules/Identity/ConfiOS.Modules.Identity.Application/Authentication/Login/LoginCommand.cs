using ConfiOS.BuildingBlocks.Application.Messaging;

namespace ConfiOS.Modules.Identity.Application.Authentication.Login;

/// <summary>
/// Signs a user in with an email and password.
/// </summary>
/// <remarks>
/// A person may hold an account at several businesses, so the credentials alone
/// do not always identify one. When they unlock exactly one business the session
/// is issued immediately; when they unlock several the caller is asked to choose.
/// <para>
/// <paramref name="BusinessHandle"/> is optional and skips the choice when the
/// caller already knows which business it wants.
/// </para>
/// </remarks>
/// <param name="Email">The user's email.</param>
/// <param name="Password">The user's password.</param>
/// <param name="BusinessHandle">Optional business handle (slug) to sign in to directly.</param>
public sealed record LoginCommand(string Email, string Password, string? BusinessHandle = null)
    : ICommand<SignInOutcome>;

/// <summary>Either a session, or the businesses to choose between.</summary>
public abstract record SignInOutcome
{
    private SignInOutcome()
    {
    }

    /// <summary>The credentials identified exactly one business.</summary>
    public sealed record Authenticated(AuthenticatedSession Session) : SignInOutcome;

    /// <summary>The credentials unlocked several businesses; the caller picks one.</summary>
    public sealed record ChoiceRequired(
        string SelectionToken,
        DateTimeOffset ExpiresAt,
        IReadOnlyList<BusinessOption> Businesses) : SignInOutcome;
}

/// <summary>The issued session and the identity behind it.</summary>
/// <param name="AccessToken">Signed JWT to send as a Bearer token.</param>
/// <param name="ExpiresAt">When the token expires.</param>
/// <param name="UserId">The signed-in user.</param>
/// <param name="TenantId">The tenant.</param>
/// <param name="BusinessName">Trading name of the tenant.</param>
/// <param name="FullName">The user's name.</param>
/// <param name="Email">The user's email.</param>
/// <param name="Permissions">Permission keys granted to the user.</param>
public sealed record AuthenticatedSession(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    Guid TenantId,
    string BusinessName,
    string FullName,
    string Email,
    IReadOnlyList<string> Permissions);

/// <summary>A business the caller may continue into.</summary>
/// <param name="TenantId">The business identifier.</param>
/// <param name="Name">Trading name.</param>
/// <param name="Slug">URL-safe handle.</param>
public sealed record BusinessOption(Guid TenantId, string Name, string Slug);
