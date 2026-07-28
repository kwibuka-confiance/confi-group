using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;

namespace ConfiOS.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// A stock quantity in a named unit. Decimal-backed so units such as litres or kilograms
/// work as well as whole crates.
/// </summary>
/// <remarks>
/// Quantities in different units are never combined arithmetically: converting crates to
/// bottles needs the catalog's unit definitions and is not a concern of this type.
/// </remarks>
public sealed class Quantity : ValueObject, IComparable<Quantity>
{
    public const string UnitMismatch = "UNIT_MISMATCH";

    private Quantity(decimal value, string unitCode)
    {
        Value = value;
        UnitCode = unitCode;
    }

    public decimal Value { get; }

    /// <summary>Unit of measure code, for example <c>EA</c>, <c>CRATE</c> or <c>L</c>.</summary>
    public string UnitCode { get; }

    public bool IsZero => Value == 0m;

    public bool IsNegative => Value < 0m;

    public static Quantity Of(decimal value, string unitCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(unitCode);
        return new Quantity(value, unitCode.Trim().ToUpperInvariant());
    }

    public static Quantity Zero(string unitCode) => Of(0m, unitCode);

    /// <summary>Creates a quantity that must be greater than zero, such as a line item quantity.</summary>
    public static Quantity Positive(decimal value, string unitCode) => value <= 0m
        ? throw new DomainException(Error.Validation(ErrorCodes.NegativeAmount))
        : Of(value, unitCode);

    public Quantity Add(Quantity other)
    {
        EnsureSameUnit(other);
        return Of(Value + other.Value, UnitCode);
    }

    public Quantity Subtract(Quantity other)
    {
        EnsureSameUnit(other);
        return Of(Value - other.Value, UnitCode);
    }

    public int CompareTo(Quantity? other)
    {
        if (other is null)
        {
            return 1;
        }

        EnsureSameUnit(other);
        return Value.CompareTo(other.Value);
    }

    public static Quantity operator +(Quantity left, Quantity right)
    {
        ArgumentNullException.ThrowIfNull(left);
        return left.Add(right);
    }

    public static Quantity operator -(Quantity left, Quantity right)
    {
        ArgumentNullException.ThrowIfNull(left);
        return left.Subtract(right);
    }

    public static bool operator <(Quantity left, Quantity right) => left.CompareTo(right) < 0;

    public static bool operator >(Quantity left, Quantity right) => left.CompareTo(right) > 0;

    public static bool operator <=(Quantity left, Quantity right) => left.CompareTo(right) <= 0;

    public static bool operator >=(Quantity left, Quantity right) => left.CompareTo(right) >= 0;

    public override string ToString() => $"{Value} {UnitCode}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return UnitCode;
    }

    private void EnsureSameUnit(Quantity other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (!string.Equals(other.UnitCode, UnitCode, StringComparison.Ordinal))
        {
            throw new DomainException(Error.Validation(
                UnitMismatch,
                new Dictionary<string, object?>
                {
                    ["expected"] = UnitCode,
                    ["actual"] = other.UnitCode,
                }));
        }
    }
}
