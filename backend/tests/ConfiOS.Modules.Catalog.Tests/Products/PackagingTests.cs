using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Catalog.Domain;
using ConfiOS.Modules.Catalog.Domain.Products;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Catalog.Tests.Products;

public sealed class PackagingTests
{
    private static readonly TenantId Tenant = TenantId.New();

    private static Money Rwf(decimal amount) => Money.Of(amount, "RWF");

    private static Product MutsigLarge() =>
        Product.Create(Tenant, "Mutsig Large 650ml", "MUT-650", Rwf(1000), "BOTTLE");

    [Theory]
    [InlineData(12, 3)]
    [InlineData(20, 5)]
    [InlineData(24, 6)]
    public void A_quarter_crate_is_a_whole_number_of_bottles(int crateSize, int expected)
    {
        var crate = Packaging.Create("CRATE", crateSize, Rwf(10000));

        crate.AllowsQuarters.ShouldBeTrue();
        crate.ToBaseUnits(0.25m).ShouldBe(expected);
    }

    [Fact]
    public void Crate_quantities_convert_to_base_units()
    {
        var crate = Packaging.Create("CRATE", 24, Rwf(20000));

        crate.ToBaseUnits(1m).ShouldBe(24);
        crate.ToBaseUnits(0.5m).ShouldBe(12);
        crate.ToBaseUnits(0.75m).ShouldBe(18);
        crate.ToBaseUnits(3m).ShouldBe(72);
    }

    [Fact]
    public void A_pack_that_does_not_divide_by_four_offers_no_quarters()
    {
        // A crate of ten would quarter into two and a half bottles.
        var crate = Packaging.Create("CRATE", 10, Rwf(5000));

        crate.AllowsQuarters.ShouldBeFalse();

        var exception = Should.Throw<DomainException>(() => crate.ToBaseUnits(0.25m));
        exception.Error.Code.ShouldBe(CatalogErrorCodes.FractionalBaseUnit);
    }

    [Fact]
    public void A_packaging_holds_at_least_one_base_unit()
    {
        Should.Throw<DomainException>(() => Packaging.Create("CRATE", 0, Rwf(100)));
    }

    [Fact]
    public void A_unit_cannot_be_defined_twice_on_one_product()
    {
        var product = MutsigLarge();
        product.AddPackaging(Packaging.Create("CRATE", 12, Rwf(11000)));

        var exception = Should.Throw<DomainException>(
            () => product.AddPackaging(Packaging.Create("CRATE", 24, Rwf(21000))));

        exception.Error.Code.ShouldBe(CatalogErrorCodes.PackagingUnitTaken);
    }

    [Fact]
    public void A_packaging_cannot_reuse_the_base_unit()
    {
        var product = MutsigLarge();

        var exception = Should.Throw<DomainException>(
            () => product.AddPackaging(Packaging.Create("BOTTLE", 1, Rwf(1000))));

        exception.Error.Code.ShouldBe(CatalogErrorCodes.PackagingIsBaseUnit);
    }

    [Fact]
    public void A_crate_may_be_priced_below_the_bottles_it_holds()
    {
        var product = MutsigLarge();
        product.AddPackaging(Packaging.Create("CRATE", 12, Rwf(11000)));

        var crate = product.FindPackaging("crate").ShouldNotBeNull();

        // Twelve bottles at 1,000 would be 12,000; the crate sells for less.
        crate.SellingPrice.Amount.ShouldBe(11000m);
        crate.QuantityInBaseUnit.ShouldBe(12);
    }

    [Fact]
    public void A_returnable_product_carries_a_deposit_per_base_unit()
    {
        var product = MutsigLarge();

        product.IsReturnable.ShouldBeFalse();

        product.MakeReturnable(Rwf(100));

        product.IsReturnable.ShouldBeTrue();
        product.DepositPerBaseUnit!.Amount.ShouldBe(100m);
    }

    [Fact]
    public void A_product_defaults_to_the_standard_tax_class()
    {
        MutsigLarge().TaxClass.ShouldBe(TaxClass.Standard);
    }
}
