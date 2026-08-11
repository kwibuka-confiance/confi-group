using ConfiOS.BuildingBlocks.Application.Auditing;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Identity.Application.Abstractions;
using ConfiOS.Modules.Identity.Domain;
using ConfiOS.Modules.Identity.Domain.Users;

namespace ConfiOS.Modules.Identity.Application.Users.InviteUser;

/// <summary>
/// Invites a user into the resolved tenant.
/// </summary>
/// <remarks>
/// The invited account starts with a random unusable password: it is replaced when the
/// person accepts the invitation, and until then no password can be guessed into working.
/// Adding a user changes who can reach the business, so it is audited (BR-008).
/// </remarks>
/// <param name="users">User repository, already scoped to the current tenant.</param>
/// <param name="roles">Role repository, used to reject unknown roles.</param>
/// <param name="tenantContext">Current tenant and acting user.</param>
/// <param name="passwordHasher">Produces the placeholder password hash.</param>
/// <param name="auditLogger">Records the invitation.</param>
/// <param name="unitOfWork">Commits the transaction.</param>
public sealed class InviteUserHandler(
    IUserRepository users,
    IRoleRepository roles,
    ITenantContext tenantContext,
    IPasswordHasher passwordHasher,
    IIdentityAuditLogger auditLogger,
    IIdentityUnitOfWork unitOfWork) : ICommandHandler<InviteUserCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(InviteUserCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var email = EmailAddress.Create(command.Email);

        if (await users.EmailExistsAsync(email.Value, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<Guid>(Error.Conflict(IdentityErrorCodes.UserEmailTaken));
        }

        foreach (var roleId in command.RoleIds)
        {
            if (await roles.GetAsync(roleId, cancellationToken).ConfigureAwait(false) is null)
            {
                return Result.Failure<Guid>(Error.NotFound(
                    IdentityErrorCodes.RoleNotFound,
                    new Dictionary<string, object?> { ["roleId"] = roleId }));
            }
        }

        var user = User.Invite(
            tenantContext.TenantId,
            email,
            command.FullName,
            passwordHasher.Hash(Guid.NewGuid().ToString("N", System.Globalization.CultureInfo.InvariantCulture)));

        foreach (var roleId in command.RoleIds)
        {
            user.AssignRole(roleId);
        }

        foreach (var branchId in command.BranchIds ?? [])
        {
            user.GrantBranchAccess(BranchId.From(branchId));
        }

        users.Add(user);

        await auditLogger.RecordAsync(
            new AuditEntry(
                "identity.user-invited",
                nameof(User),
                user.Id,
                tenantContext.TenantId,
                tenantContext.BranchId,
                tenantContext.UserId,
                new Dictionary<string, object?>
                {
                    ["email"] = email.Value,
                    ["roleIds"] = command.RoleIds,
                }),
            cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(user.Id);
    }
}
