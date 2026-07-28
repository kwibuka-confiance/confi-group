using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Application.Context;

/// <summary>
/// Mutable tenant context populated by the transport layer, then read as
/// <see cref="ITenantContext"/> by everything downstream. Registered per request.
/// </summary>
public sealed class AmbientContext : ITenantContext
{
    private TenantId? _tenantId;

    public bool IsResolved => _tenantId.HasValue;

    public TenantId TenantId => _tenantId
        ?? throw new InvalidOperationException(
            "No tenant has been resolved for this operation. Requests must pass through tenant resolution, and background work must open an explicit tenant scope.");

    public BranchId? BranchId { get; private set; }

    public UserId? UserId { get; private set; }

    public string Locale { get; private set; } = "en";

    public string CurrencyCode { get; private set; } = "RWF";

    public string TimeZoneId { get; private set; } = "Africa/Kigali";

    public void Resolve(
        TenantId tenantId,
        UserId? userId,
        BranchId? branchId,
        string locale,
        string currencyCode,
        string timeZoneId)
    {
        _tenantId = tenantId;
        UserId = userId;
        BranchId = branchId;
        Locale = string.IsNullOrWhiteSpace(locale) ? "en" : locale;
        CurrencyCode = string.IsNullOrWhiteSpace(currencyCode) ? "RWF" : currencyCode;
        TimeZoneId = string.IsNullOrWhiteSpace(timeZoneId) ? "Africa/Kigali" : timeZoneId;
    }
}
