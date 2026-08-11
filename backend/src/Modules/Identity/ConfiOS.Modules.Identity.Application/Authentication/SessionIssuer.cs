using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Application.Authentication.Login;
using ConfiOS.Modules.Identity.Domain.Tenants;
using ConfiOS.Modules.Identity.Domain.Users;

namespace ConfiOS.Modules.Identity.Application.Authentication;

/// <summary>
/// Turns a verified user into a session. Shared by signing in directly and by
/// choosing a business, so both paths issue identical tokens.
/// </summary>
/// <param name="permissions">Gathers the user's permissions for the token.</param>
/// <param name="tokenGenerator">Signs the access token.</param>
/// <param name="clock">Records the sign-in time.</param>
/// <param name="unitOfWork">Persists the sign-in timestamp.</param>
public sealed class SessionIssuer(
    IPermissionService permissions,
    IAccessTokenGenerator tokenGenerator,
    IClock clock,
    IIdentityUnitOfWork unitOfWork)
{
    public async Task<AuthenticatedSession> IssueAsync(
        Tenant tenant,
        User user,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        ArgumentNullException.ThrowIfNull(user);

        var userId = UserId.From(user.Id);
        var granted = await permissions
            .GetPermissionsAsync(userId, tenant.TenantId, cancellationToken)
            .ConfigureAwait(false);

        var token = tokenGenerator.Generate(new AccessTokenClaims(
            tenant.TenantId,
            userId,
            user.Email.Value,
            tenant.Settings.Currency.Code,
            tenant.Settings.TimeZoneId,
            granted));

        user.RecordSignIn(clock.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new AuthenticatedSession(
            token.Value,
            token.ExpiresAt,
            user.Id,
            tenant.Id,
            tenant.Name,
            user.FullName,
            user.Email.Value,
            granted);
    }
}
