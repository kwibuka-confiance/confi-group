using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace ConfiOS.BuildingBlocks.Tests.ValueObjects;

public sealed class MoneyTests
{
    [Fact]
    public void Rounds_to_the_currency_minor_units()
    {
        // RWF has no minor units, so fractional francs are not representable.
        Money.Of(1250.49m, "RWF").Amount.ShouldBe(1250m);
        Money.Of(12.504m, "USD").Amount.ShouldBe(12.50m);
    }

    [Fact]
    public void Adding_a_different_currency_is_rejected()
    {
        var francs = Money.Of(1000m, "RWF");
        var dollars = Money.Of(10m, "USD");

        var exception = Should.Throw<DomainException>(() => francs.Add(dollars));

        exception.Error.Code.ShouldBe(ErrorCodes.CurrencyMismatch);
    }

    [Fact]
    public void Adds_and_subtracts_within_one_currency()
    {
        var total = Money.Of(1000m, "RWF") + Money.Of(250m, "RWF") - Money.Of(50m, "RWF");

        total.Amount.ShouldBe(1200m);
        total.Currency.Code.ShouldBe("RWF");
    }

    [Fact]
    public void Equality_covers_amount_and_currency()
    {
        // Assert value equality directly rather than through Shouldly's IComparable path, so
        // this test exercises Equals (amount and currency) rather than the ordering CompareTo.
        Money.Of(100m, "RWF").Equals(Money.Of(100m, "RWF")).ShouldBeTrue();
        Money.Of(100m, "RWF").Equals(Money.Of(100m, "USD")).ShouldBeFalse();
    }

    [Fact]
    public void Orders_within_a_currency_by_amount()
    {
        Money.Of(50m, "RWF").CompareTo(Money.Of(100m, "RWF")).ShouldBeLessThan(0);
        (Money.Of(100m, "RWF") > Money.Of(50m, "RWF")).ShouldBeTrue();
    }

    [Fact]
    public void Orders_across_currencies_without_throwing()
    {
        // A total order keeps Money usable in sorts and comparer-based collections; mixed
        // currencies are ordered by currency code, never rejected.
        var sorted = new[]
        {
            Money.Of(10m, "USD"),
            Money.Of(5m, "RWF"),
            Money.Of(1m, "USD"),
        }
        .OrderBy(money => money)
        .ToArray();

        sorted.ShouldBe(new[]
        {
            Money.Of(5m, "RWF"),
            Money.Of(1m, "USD"),
            Money.Of(10m, "USD"),
        });
    }

    [Fact]
    public void NonNegative_rejects_a_negative_amount()
    {
        var exception = Should.Throw<DomainException>(
            () => Money.NonNegative(-1m, Currency.Rwf));

        exception.Error.Code.ShouldBe(ErrorCodes.NegativeAmount);
    }

    [Fact]
    public void Unsupported_currency_is_rejected()
    {
        var exception = Should.Throw<DomainException>(() => Money.Of(1m, "XYZ"));

        exception.Error.Code.ShouldBe(ErrorCodes.UnsupportedCurrency);
    }

    [Fact]
    public void Multiplying_keeps_the_currency_and_rounds()
    {
        var line = Money.Of(1500m, "RWF").Multiply(3.5m);

        line.Amount.ShouldBe(5250m);
        line.Currency.Code.ShouldBe("RWF");
    }
}
