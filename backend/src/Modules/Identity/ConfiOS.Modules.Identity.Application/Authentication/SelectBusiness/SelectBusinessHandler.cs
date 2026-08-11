using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Application.Authentication.Login;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Users;

namespace ConfiOS.Modules.Identity.Application.Authentication.SelectBusiness;

/// <summary>
/// Issues a session for the business the caller picked after signing in.
/// </summary>
/// <remarks>
/// The password is not re-checked here; the selection token is proof it already
/// passed. The chosen business must be one the token names, so a caller cannot
/// swap in a business their credentials never unlocked.
/// </remarks>
/// <param name="tenants">Loads the chosen business.</param>
/// <param name="users">Loads the account within it.</param>
/// <param name="selectionTokens">Validates the selection token.</param>
/// <param name="sessionIssuer">Issues the session.</param>
public sealed class SelectBusinessHandler(
    ITenantRepository tenants,
    IUserRepository users,
    IBusinessSelectionTokens selectionTokens,
    SessionIssuer sessionIssuer) : ICommandHandler<SelectBusinessCommand, AuthenticatedSession>
{
    public async Task<Result<AuthenticatedSession>> HandleAsync(
        SelectBusinessCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var invalid = Result.Failure<AuthenticatedSession>(
            Error.Unauthorized(IdentityErrorCodes.InvalidCredentials));

        var payload = selectionTokens.Validate(command.SelectionToken);
        if (payload is null || !payload.TenantIds.Contains(command.TenantId))
        {
            return invalid;
        }

        var tenantId = TenantId.From(command.TenantId);
        var tenant = await tenants.GetAsync(tenantId, cancellationToken).ConfigureAwait(false);
        if (tenant is null)
        {
            return invalid;
        }

        var user = await users
            .GetForAuthenticationAsync(tenantId, payload.Email, cancellationToken)
            .ConfigureAwait(false);

        if (user is null || user.Status != UserStatus.Active)
        {
            return invalid;
        }

        var session = await sessionIssuer.IssueAsync(tenant, user, cancellationToken).ConfigureAwait(false);
        return Result.Success(session);
    }
}
