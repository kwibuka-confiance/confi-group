using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Tenants;
using ConfiOS.Modules.Identity.Domain.Users;

namespace ConfiOS.Modules.Identity.Application.Authentication.Login;

/// <summary>
/// Verifies credentials and either issues a session or asks which business to
/// continue into.
/// </summary>
/// <remarks>
/// Accounts are per business, so one email can exist several times with its own
/// password each. The password is therefore checked against every account using
/// that email, and only the businesses it actually unlocks are offered — a
/// password for one business never reveals another.
/// <para>
/// Every failure returns the same <c>INVALID_CREDENTIALS</c>, so sign-in cannot be
/// used to discover which businesses, emails or accounts exist.
/// </para>
/// </remarks>
/// <param name="tenants">Resolves businesses, above the tenant filter.</param>
/// <param name="users">Finds the accounts using an email.</param>
/// <param name="passwordHasher">Verifies the password against each stored hash.</param>
/// <param name="selectionTokens">Carries the completed password check to the second step.</param>
/// <param name="sessionIssuer">Issues the session once one business is settled on.</param>
public sealed class LoginHandler(
    ITenantRepository tenants,
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IBusinessSelectionTokens selectionTokens,
    SessionIssuer sessionIssuer) : ICommandHandler<LoginCommand, SignInOutcome>
{
    public async Task<Result<SignInOutcome>> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var invalid = Result.Failure<SignInOutcome>(
            Error.Unauthorized(IdentityErrorCodes.InvalidCredentials));

        var email = command.Email.Trim().ToLowerInvariant();

        var candidates = await users
            .FindByEmailAcrossBusinessesAsync(email, cancellationToken)
            .ConfigureAwait(false);

        // Only accounts whose own password matches are ever revealed.
        var verified = candidates
            .Where(user => user.Status == UserStatus.Active)
            .Where(user => passwordHasher.Verify(command.Password, user.PasswordHash))
            .ToList();

        if (verified.Count == 0)
        {
            return invalid;
        }

        var businesses = await tenants
            .GetManyAsync(verified.Select(user => user.TenantId).ToList(), cancellationToken)
            .ConfigureAwait(false);

        var byTenant = businesses.ToDictionary(tenant => tenant.Id);

        // A handle was supplied, so there is nothing to choose.
        if (!string.IsNullOrWhiteSpace(command.BusinessHandle))
        {
            var handle = command.BusinessHandle.Trim().ToLowerInvariant();
            var match = businesses.FirstOrDefault(tenant =>
                string.Equals(tenant.Slug, handle, StringComparison.Ordinal));

            if (match is null)
            {
                return invalid;
            }

            return await AuthenticateAsync(match, verified, cancellationToken).ConfigureAwait(false);
        }

        if (verified.Count == 1 && byTenant.TryGetValue(verified[0].TenantId, out var only))
        {
            return await AuthenticateAsync(only, verified, cancellationToken).ConfigureAwait(false);
        }

        var selection = selectionTokens.Issue(email, verified.Select(user => user.TenantId).ToList());

        IReadOnlyList<BusinessOption> options = businesses
            .OrderBy(tenant => tenant.Name, StringComparer.OrdinalIgnoreCase)
            .Select(tenant => new BusinessOption(tenant.Id, tenant.Name, tenant.Slug))
            .ToList();

        return Result.Success<SignInOutcome>(
            new SignInOutcome.ChoiceRequired(selection.Value, selection.ExpiresAt, options));
    }

    private async Task<Result<SignInOutcome>> AuthenticateAsync(
        Tenant tenant,
        IReadOnlyList<User> verified,
        CancellationToken cancellationToken)
    {
        var user = verified.First(candidate => candidate.TenantId == tenant.Id);
        var session = await sessionIssuer.IssueAsync(tenant, user, cancellationToken).ConfigureAwait(false);

        return Result.Success<SignInOutcome>(new SignInOutcome.Authenticated(session));
    }
}
