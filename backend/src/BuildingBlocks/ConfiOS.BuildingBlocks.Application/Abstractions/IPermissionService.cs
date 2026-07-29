using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Answers whether the current user may perform an action in the resolved tenant.
/// Implemented by the Identity module; other modules depend on this contract, never on
/// Identity's tables (MT-003).
/// </summary>
public interface IPermissionService
{
    Task<bool> HasPermissionAsync(
        UserId userId,
        TenantId tenantId,
        string permission,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms the user is assigned to the branch and that the branch belongs to the
    /// tenant. Branch access never crosses a tenant boundary (MT-005).
    /// </summary>
    Task<bool> HasBranchAccessAsync(
        UserId userId,
        TenantId tenantId,
        BranchId branchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Every permission key granted to the user in the tenant, used to populate the access
    /// token issued at sign-in.
    /// </summary>
    Task<IReadOnlyList<string>> GetPermissionsAsync(
        UserId userId,
        TenantId tenantId,
        CancellationToken cancellationToken = default);
}
