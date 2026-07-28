using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Application.Context;

/// <summary>
/// The tenant, branch and user the current request belongs to (MT-002).
/// </summary>
/// <remarks>
/// Resolved once at the edge from validated identity claims. A tenant identifier sent by
/// a client is never trusted on its own: membership is checked before this is populated
/// (docs/03-architecture/05-api-standards.md).
/// </remarks>
public interface ITenantContext
{
    /// <summary>True once a tenant has been resolved for this request.</summary>
    bool IsResolved { get; }

    /// <summary>The tenant. Throws when no tenant has been resolved.</summary>
    TenantId TenantId { get; }

    /// <summary>The branch, when the request is scoped to one.</summary>
    BranchId? BranchId { get; }

    /// <summary>The acting user, absent for background and system work.</summary>
    UserId? UserId { get; }

    /// <summary>BCP 47 tag for the request, for example <c>rw-RW</c>.</summary>
    string Locale { get; }

    /// <summary>Tenant's default ISO 4217 currency.</summary>
    string CurrencyCode { get; }

    /// <summary>IANA time zone of the tenant, for example <c>Africa/Kigali</c>.</summary>
    string TimeZoneId { get; }
}
