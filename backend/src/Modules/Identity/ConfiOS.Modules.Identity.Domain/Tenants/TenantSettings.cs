using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;

namespace ConfiOS.Modules.Identity.Domain.Tenants;

/// <summary>
/// The country and locale configuration a tenant operates under
/// (docs/02-product/04-multi-tenant-architecture.md).
/// </summary>
/// <remarks>
/// Held as a value object rather than loose columns because these settings are read
/// together on every request and only ever change as a set.
/// </remarks>
public sealed class TenantSettings : ValueObject
{
    /// <summary>Languages the MVP supports (CLAUDE.md).</summary>
    public static readonly string[] SupportedLanguages = ["en", "rw", "fr"];

    private TenantSettings(
        string countryCode,
        Currency currency,
        string defaultLanguage,
        string timeZoneId,
        int fiscalYearStartMonth)
    {
        CountryCode = countryCode;
        Currency = currency;
        DefaultLanguage = defaultLanguage;
        TimeZoneId = timeZoneId;
        FiscalYearStartMonth = fiscalYearStartMonth;
    }

    private TenantSettings()
    {
    }

    /// <summary>ISO 3166-1 alpha-2 country code.</summary>
    public string CountryCode { get; private set; } = "RW";

    public Currency Currency { get; private set; } = Currency.Rwf;

    /// <summary>BCP 47 language tag used when a user has expressed no preference.</summary>
    public string DefaultLanguage { get; private set; } = "en";

    /// <summary>IANA time zone, used for business-day boundaries and reports.</summary>
    public string TimeZoneId { get; private set; } = "Africa/Kigali";

    /// <summary>Month the fiscal year starts in, 1 to 12.</summary>
    public int FiscalYearStartMonth { get; private set; } = 1;

    public static TenantSettings Create(
        string countryCode,
        string currencyCode,
        string defaultLanguage,
        string timeZoneId,
        int fiscalYearStartMonth = 1)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultLanguage);
        ArgumentException.ThrowIfNullOrWhiteSpace(timeZoneId);

        var language = defaultLanguage.Trim().ToLowerInvariant();

        if (!SupportedLanguages.Contains(language, StringComparer.Ordinal))
        {
            throw new DomainException(Error.Validation(
                IdentityErrorCodes.UnsupportedLanguage,
                new Dictionary<string, object?>
                {
                    ["language"] = language,
                    ["supported"] = SupportedLanguages,
                }));
        }

        if (fiscalYearStartMonth is < 1 or > 12)
        {
            throw new DomainException(Error.Validation(ErrorCodes.ValidationFailed));
        }

        return new TenantSettings(
            countryCode.Trim().ToUpperInvariant(),
            Currency.FromCode(currencyCode),
            language,
            timeZoneId.Trim(),
            fiscalYearStartMonth);
    }

    /// <summary>Defaults for a Rwandan business, the first market ConfiOS serves.</summary>
    public static TenantSettings Default() => Create("RW", "RWF", "en", "Africa/Kigali");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return Currency;
        yield return DefaultLanguage;
        yield return TimeZoneId;
        yield return FiscalYearStartMonth;
    }
}
