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
