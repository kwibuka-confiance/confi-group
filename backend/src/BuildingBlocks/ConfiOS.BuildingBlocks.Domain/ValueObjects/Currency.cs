using ConfiOS.BuildingBlocks.Domain.Errors;

namespace ConfiOS.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// An ISO 4217 currency, per rule 9 in CLAUDE.md.
/// </summary>
/// <remarks>
/// <see cref="MinorUnits"/> drives rounding: RWF has none, so 1250.4 RWF is not a
/// representable amount, while 12.504 USD rounds to 12.50.
/// </remarks>
public sealed class Currency : ValueObject
{
    private static readonly Dictionary<string, Currency> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        ["RWF"] = new("RWF", 0),
        ["USD"] = new("USD", 2),
        ["EUR"] = new("EUR", 2),
        ["KES"] = new("KES", 2),
        ["UGX"] = new("UGX", 0),
        ["TZS"] = new("TZS", 2),
    };

    private Currency(string code, int minorUnits)
    {
        Code = code;
        MinorUnits = minorUnits;
    }

    /// <summary>Three-letter ISO 4217 code, always upper case.</summary>
    public string Code { get; }

    /// <summary>Number of decimal places the currency is denominated in.</summary>
    public int MinorUnits { get; }

    public static Currency Rwf => Supported["RWF"];

    public static IReadOnlyCollection<Currency> All => Supported.Values;

    public static Currency FromCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        return Supported.TryGetValue(code, out var currency)
            ? currency
            : throw new DomainException(Error.Validation(
                ErrorCodes.UnsupportedCurrency,
                new Dictionary<string, object?> { ["currency"] = code }));
    }

    public static bool IsSupported(string code) =>
        !string.IsNullOrWhiteSpace(code) && Supported.ContainsKey(code);

    public override string ToString() => Code;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }
}
