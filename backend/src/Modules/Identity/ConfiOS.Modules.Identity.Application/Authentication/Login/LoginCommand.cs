using ConfiOS.BuildingBlocks.Application.Messaging;

namespace ConfiOS.Modules.Identity.Application.Authentication.Login;

/// <summary>
/// Signs a user in. The business handle selects the tenant (a user belongs to exactly one),
/// then the email and password are checked within it.
/// </summary>
/// <param name="BusinessHandle">The tenant's URL-safe handle (slug).</param>
/// <param name="Email">The user's email.</param>
/// <param name="Password">The user's password.</param>
public sealed record LoginCommand(string BusinessHandle, string Email, string Password)
    : ICommand<LoginResult>;

/// <summary>The issued session and the identity behind it.</summary>
/// <param name="AccessToken">Signed JWT to send as a Bearer token.</param>
/// <param name="ExpiresAt">When the token expires.</param>
/// <param name="UserId">The signed-in user.</param>
/// <param name="TenantId">The tenant.</param>
/// <param name="BusinessName">Trading name of the tenant.</param>
/// <param name="FullName">The user's name.</param>
/// <param name="Email">The user's email.</param>
/// <param name="Permissions">Permission keys granted to the user.</param>
public sealed record LoginResult(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    Guid TenantId,
    string BusinessName,
    string FullName,
    string Email,
    IReadOnlyList<string> Permissions);
