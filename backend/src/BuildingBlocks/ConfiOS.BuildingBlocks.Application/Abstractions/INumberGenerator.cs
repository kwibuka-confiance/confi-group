using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Application.Abstractions;

/// <summary>
/// Issues human-facing document numbers such as <c>INV-2026-000042</c>.
/// </summary>
/// <remarks>
/// Sequences are per tenant and per series, and must have no gaps because tax authorities
/// and auditors read them. Implementations therefore allocate inside the caller's
/// transaction rather than from a cache.
/// </remarks>
public interface INumberGenerator
{
    Task<string> NextAsync(
        TenantId tenantId,
        string series,
        CancellationToken cancellationToken = default);
}
