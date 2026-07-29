using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.Modules.Identity.Infrastructure.Services;

/// <summary>
/// Answers authorization questions for every module.
/// </summary>
/// <remarks>
/// Both checks re-assert the tenant boundary in the query itself rather than trusting the
/// caller's tenant argument. Branch access in particular is checked against the branch's
/// own tenant, so a branch identifier from another business fails even if the user holds
/// the matching permission (MT-005).
/// Reads are uncached for now. If this becomes hot, cache per user and version the key on
/// role change; do not cache the tenant check itself.
/// </remarks>
/// <param name="context">Identity database context.</param>
public sealed class PermissionService(IdentityDbContext context) : IPermissionService
{
    public async Task<bool> HasPermissionAsync(
        UserId userId,
        TenantId tenantId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        var roleIds = await context.Users
            .IgnoreQueryFilters()
            .Where(user => user.Id == userId.Value
                && user.TenantId == tenantId.Value
                && !user.IsDeleted
                && user.Status == Domain.Users.UserStatus.Active)
            .SelectMany(user => user.Roles.Select(role => role.RoleId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (roleIds.Count == 0)
        {
            return false;
        }

        return await context.Roles
            .IgnoreQueryFilters()
            .Where(role => roleIds.Contains(role.Id) && role.TenantId == tenantId.Value)
            .AnyAsync(
                role => role.Permissions.Any(item => item.Permission == permission),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(
        UserId userId,
        TenantId tenantId,
        CancellationToken cancellationToken = default)
    {
        var roleIds = await context.Users
            .IgnoreQueryFilters()
            .Where(user => user.Id == userId.Value
                && user.TenantId == tenantId.Value
                && !user.IsDeleted
                && user.Status == Domain.Users.UserStatus.Active)
            .SelectMany(user => user.Roles.Select(role => role.RoleId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (roleIds.Count == 0)
        {
            return [];
        }

        return await context.Roles
            .IgnoreQueryFilters()
            .Where(role => roleIds.Contains(role.Id) && role.TenantId == tenantId.Value)
            .SelectMany(role => role.Permissions.Select(item => item.Permission))
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public Task<bool> HasBranchAccessAsync(
        UserId userId,
        TenantId tenantId,
        BranchId branchId,
        CancellationToken cancellationToken = default) =>
        context.Users
            .IgnoreQueryFilters()
            .Where(user => user.Id == userId.Value && user.TenantId == tenantId.Value && !user.IsDeleted)
            .AnyAsync(
                user => user.Branches.Any(assignment => assignment.BranchId == branchId.Value)
                    && context.Branches.Any(branch =>
                        branch.Id == branchId.Value && branch.TenantId == tenantId.Value),
                cancellationToken);
}
