using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Users;

namespace ConfiOS.Modules.Identity.Application.Authentication.Login;

/// <summary>
/// Verifies a user's credentials and issues an access token.
/// </summary>
/// <remarks>
/// Every failure — unknown business, unknown user, deactivated account, wrong password —
/// returns the same <c>INVALID_CREDENTIALS</c> so sign-in cannot be used to discover which
/// businesses, emails or accounts exist.
/// </remarks>
/// <param name="tenants">Resolves the tenant from its handle, above the tenant filter.</param>
/// <param name="users">Loads the user for authentication.</param>
/// <param name="passwordHasher">Verifies the password against the stored hash.</param>
/// <param name="permissions">Gathers the user's permissions for the token.</param>
/// <param name="tokenGenerator">Signs the access token.</param>
/// <param name="clock">Records the sign-in time.</param>
/// <param name="unitOfWork">Persists the sign-in timestamp.</param>
public sealed class LoginHandler(
    ITenantRepository tenants,
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IPermissionService permissions,
    IAccessTokenGenerator tokenGenerator,
    IClock clock,
    IUnitOfWork unitOfWork) : ICommandHandler<LoginCommand, LoginResult>
{
    public async Task<Result<LoginResult>> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var invalid = Result.Failure<LoginResult>(
            Error.Unauthorized(IdentityErrorCodes.InvalidCredentials));

        var handle = command.BusinessHandle.Trim().ToLowerInvariant();
        var tenant = await tenants.GetBySlugAsync(handle, cancellationToken).ConfigureAwait(false);
        if (tenant is null)
        {
            return invalid;
        }

        var email = command.Email.Trim().ToLowerInvariant();
        var user = await users
            .GetForAuthenticationAsync(tenant.TenantId, email, cancellationToken)
            .ConfigureAwait(false);

        if (user is null || user.Status != UserStatus.Active)
        {
            return invalid;
        }

        if (!passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            return invalid;
        }

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

        return Result.Success(new LoginResult(
            token.Value,
            token.ExpiresAt,
            user.Id,
            tenant.Id,
            tenant.Name,
            user.FullName,
            user.Email.Value,
            granted));
    }
}
