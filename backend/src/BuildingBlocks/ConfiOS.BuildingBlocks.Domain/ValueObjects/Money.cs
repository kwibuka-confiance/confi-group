using System.Globalization;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// An amount in a specific currency. Backed by <see cref="decimal"/>: floating point is
/// never used for money (docs/03-architecture/04-database-design.md).
/// </summary>
/// <remarks>
/// Arithmetic across currencies is rejected rather than silently coerced. Converting
/// between currencies is an explicit operation that needs a rate, so it does not belong
/// on this type.
/// </remarks>
public sealed class Money : ValueObject, IComparable<Money>
{
    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public Currency Currency { get; }

    public bool IsZero => Amount == 0m;

    public bool IsNegative => Amount < 0m;

    public static Money Zero(Currency currency) => new(0m, currency);

    public static Money Of(decimal amount, Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);
        return new Money(Math.Round(amount, currency.MinorUnits, MidpointRounding.ToEven), currency);
    }

    public static Money Of(decimal amount, string currencyCode) =>
        Of(amount, Currency.FromCode(currencyCode));

    /// <summary>Creates an amount that must not be negative, such as a price or a quantity value.</summary>
    public static Money NonNegative(decimal amount, Currency currency)
    {
        var money = Of(amount, currency);
        return money.IsNegative
            ? throw new DomainException(Error.Validation(ErrorCodes.NegativeAmount))
            : money;
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return Of(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return Of(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor) => Of(Amount * factor, Currency);

    public Money Negate() => Of(-Amount, Currency);

    public int CompareTo(Money? other)
    {
        if (other is null)
        {
            return 1;
        }

        EnsureSameCurrency(other);
        return Amount.CompareTo(other.Amount);
    }

    /// <summary>
    /// Formats for display in the supplied culture. Presentation only: never parse this
    /// back, and never store it.
    /// </summary>
    public string Format(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);
        return string.Create(
            culture,
            $"{Amount.ToString("N" + Currency.MinorUnits.ToString(CultureInfo.InvariantCulture), culture)} {Currency.Code}");
    }

    public override string ToString() => Format(CultureInfo.InvariantCulture);

    public static Money operator +(Money left, Money right)
    {
        ArgumentNullException.ThrowIfNull(left);
        return left.Add(right);
    }

    public static Money operator -(Money left, Money right)
    {
        ArgumentNullException.ThrowIfNull(left);
        return left.Subtract(right);
    }

    public static Money operator *(Money left, decimal factor)
    {
        ArgumentNullException.ThrowIfNull(left);
        return left.Multiply(factor);
    }

    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;

    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;

    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;

    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    private void EnsureSameCurrency(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (other.Currency != Currency)
        {
            throw new DomainException(Error.Validation(
                ErrorCodes.CurrencyMismatch,
                new Dictionary<string, object?>
                {
                    ["expected"] = Currency.Code,
                    ["actual"] = other.Currency.Code,
                }));
        }
    }
}
