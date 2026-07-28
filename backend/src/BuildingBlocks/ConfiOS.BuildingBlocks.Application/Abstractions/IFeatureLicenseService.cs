using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Reports whether a tenant's subscription and feature flags enable a module (MT-006).
/// Checked in the request pipeline alongside authentication and permissions.
/// </summary>
public interface IFeatureLicenseService
{
    Task<bool> IsEnabledAsync(
        TenantId tenantId,
        string featureKey,
        CancellationToken cancellationToken = default);
}
