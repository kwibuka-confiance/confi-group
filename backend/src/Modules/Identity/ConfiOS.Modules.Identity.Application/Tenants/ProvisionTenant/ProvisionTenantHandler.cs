using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Authorization;
using ConfiOS.Modules.Identity.Domain.Tenants;
using ConfiOS.Modules.Identity.Domain.Users;

namespace ConfiOS.Modules.Identity.Application.Tenants.ProvisionTenant;

/// <summary>
/// Provisions a business in one transaction: tenant, first branch, system roles and owner.
/// </summary>
/// <remarks>
/// All of it commits together. A tenant with no owner would be unreachable, and a tenant
/// with no roles would reject every subsequent authorization check, so a partial success
/// here is worse than a clean failure.
/// </remarks>
/// <param name="tenants">Tenant repository.</param>
/// <param name="users">User repository.</param>
/// <param name="roles">Role repository.</param>
/// <param name="passwordHasher">Hashes the owner's initial password.</param>
/// <param name="unitOfWork">Commits the transaction.</param>
public sealed class ProvisionTenantHandler(
    ITenantRepository tenants,
    IUserRepository users,
    IRoleRepository roles,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : ICommandHandler<ProvisionTenantCommand, ProvisionTenantResult>
{
    public async Task<Result<ProvisionTenantResult>> HandleAsync(
        ProvisionTenantCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var slug = command.Slug.Trim().ToLowerInvariant();

        if (await tenants.SlugExistsAsync(slug, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<ProvisionTenantResult>(
                Error.Conflict(IdentityErrorCodes.TenantSlugTaken));
        }

        var settings = TenantSettings.Create(
            command.CountryCode,
            command.CurrencyCode,
            command.DefaultLanguage,
            command.TimeZoneId);

        var tenant = Tenant.Register(command.Name, slug, settings);
        var branch = tenant.AddBranch(command.FirstBranchName, "HQ", address: null);

        tenants.Add(tenant);

        var ownerRole = CreateSystemRoles(tenant);

        var passwordHash = passwordHasher.Hash(command.OwnerPassword);

        var owner = User.Invite(
            tenant.TenantId,
            EmailAddress.Create(command.OwnerEmail),
            command.OwnerFullName,
            passwordHash);

        // The owner chose the password during provisioning, so there is no invitation to accept.
        owner.Activate(passwordHash);
        owner.AssignRole(ownerRole.Id);
        owner.GrantBranchAccess(BranchId.From(branch.Id));
        owner.ChangeLanguage(settings.DefaultLanguage);

        users.Add(owner);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(new ProvisionTenantResult(tenant.Id, branch.Id, owner.Id));
    }

    /// <summary>
    /// Seeds the four system roles. Owner receives every permission the platform defines,
    /// including ones added by modules loaded later.
    /// </summary>
    private Role CreateSystemRoles(Tenant tenant)
    {
        Role? ownerRole = null;

        foreach (var name in SystemRoles.All)
        {
            var role = Role.Create(tenant.TenantId, name, isSystemRole: true);

            if (string.Equals(name, SystemRoles.Owner, StringComparison.Ordinal))
            {
                role.GrantAll(Permissions.All);
                ownerRole = role;
            }

            roles.Add(role);
        }

        return ownerRole
            ?? throw new InvalidOperationException("The owner role is missing from SystemRoles.All.");
    }
}
