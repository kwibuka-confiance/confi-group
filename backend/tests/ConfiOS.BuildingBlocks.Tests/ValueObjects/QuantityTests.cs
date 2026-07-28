using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ConfiOS.BuildingBlocks.Tests.ValueObjects;

public sealed class QuantityTests
{
    [Fact]
    public void Combining_different_units_is_rejected()
    {
        var crates = Quantity.Of(5m, "CRATE");
        var bottles = Quantity.Of(120m, "EA");

        var exception = Should.Throw<DomainException>(() => crates.Add(bottles));

        exception.Error.Code.ShouldBe(Quantity.UnitMismatch);
    }

    [Fact]
    public void Unit_codes_are_normalised_to_upper_case()
    {
        Quantity.Of(1m, "crate").UnitCode.ShouldBe("CRATE");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Positive_rejects_zero_and_negative_values(decimal value)
    {
        Should.Throw<DomainException>(() => Quantity.Positive(value, "EA"));
    }

    [Fact]
    public void Orders_within_a_unit_by_value()
    {
        Quantity.Of(3m, "EA").CompareTo(Quantity.Of(8m, "EA")).ShouldBeLessThan(0);
        (Quantity.Of(8m, "EA") > Quantity.Of(3m, "EA")).ShouldBeTrue();
    }

    [Fact]
    public void Orders_across_units_without_throwing()
    {
        // A total order keeps Quantity usable in sorts and comparer-based collections; mixed
        // units are ordered by unit code, never rejected.
        var sorted = new[]
        {
            Quantity.Of(2m, "EA"),
            Quantity.Of(5m, "CRATE"),
            Quantity.Of(1m, "EA"),
        }
        .OrderBy(quantity => quantity)
        .ToArray();

        sorted.ShouldBe(new[]
        {
            Quantity.Of(5m, "CRATE"),
            Quantity.Of(1m, "EA"),
            Quantity.Of(2m, "EA"),
        });
    }

    [Fact]
    public void Subtracting_below_zero_is_allowed_on_the_value_object()
    {
        // The invariant that stock may not go negative (BR-003) belongs to the inventory
        // aggregate, not here: an adjustment calculation may legitimately pass through a
        // negative intermediate value.
        var result = Quantity.Of(5m, "EA") - Quantity.Of(8m, "EA");

        result.Value.ShouldBe(-3m);
        result.IsNegative.ShouldBeTrue();
    }
}
