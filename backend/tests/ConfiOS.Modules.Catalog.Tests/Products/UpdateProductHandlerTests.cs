using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Catalog.Application.Products.CreateProduct;
using ConfiOS.Modules.Catalog.Application.Products.UpdateProduct;
using ConfiOS.Modules.Catalog.Domain;
using ConfiOS.Modules.Catalog.Domain.Products;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Catalog.Tests.Products;

public sealed class UpdateProductHandlerTests
{
    private static Product Existing()
    {
        var product = Product.Create(
            ConfiOS.BuildingBlocks.Domain.Primitives.TenantId.New(),
            "Mutsig Small 330ml",
            "MUT-330",
            Money.Of(800, Currency.FromCode("RWF")),
            "BOTTLE");

        product.MakeReturnable(Money.Of(100, Currency.FromCode("RWF")));
        product.AddPackaging(Packaging.Create("CRATE", 24, Money.Of(18000, Currency.FromCode("RWF"))));
        return product;
    }

    private static UpdateProductCommand Command(
        Guid productId,
        decimal price = 900,
        decimal? deposit = 100,
        IReadOnlyList<PackagingInput>? packagings = null) => new(
        productId,
        "Mutsig Small 330ml",
        "MUT-330",
        PriceAmount: price,
        CurrencyCode: "RWF",
        Description: "Updated",
        CostAmount: 650,
        TaxClass: "Standard",
        DepositAmount: deposit,
        Packagings: packagings ?? [new PackagingInput("CRATE", 24, 21000)]);

    private static (UpdateProductHandler Handler, FakeProductRepository Repository, FakeAuditLogger Audit)
        Build(FakeProductRepository? repository = null)
    {
        var repo = repository ?? new FakeProductRepository();
        var audit = new FakeAuditLogger();
        return (new UpdateProductHandler(repo, new FakeTenantContext(), audit, new FakeUnitOfWork()), repo, audit);
    }

    [Fact]
    public async Task A_price_change_is_applied_and_audited_with_both_values()
    {
        var (handler, repository, audit) = Build();
        var product = repository.Seed(Existing());

        var result = await handler.HandleAsync(Command(product.Id, price: 900), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        product.Price.Amount.ShouldBe(900m);

        var entry = audit.Entries.ShouldHaveSingleItem();
        entry.Action.ShouldBe("catalog.product-updated");
        entry.Summary!["priceBefore"].ShouldBe(800m);
        entry.Summary["priceAfter"].ShouldBe(900m);
    }

    [Fact]
    public async Task A_missing_product_is_reported_as_not_found()
    {
        var (handler, _, audit) = Build();

        var result = await handler.HandleAsync(Command(Guid.CreateVersion7()), CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(CatalogErrorCodes.ProductNotFound);
        audit.Entries.ShouldBeEmpty();
    }

    [Fact]
    public async Task A_product_keeps_its_own_sku_without_clashing_with_itself()
    {
        var (handler, repository, _) = Build();
        var product = repository.Seed(Existing());

        var result = await handler.HandleAsync(Command(product.Id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        repository.LastSkuExclusion.ShouldBe(product.Id);
    }

    [Fact]
    public async Task A_sku_held_by_another_product_is_rejected()
    {
        var (handler, repository, _) = Build(new FakeProductRepository { SkuTaken = true });
        var product = repository.Seed(Existing());

        var result = await handler.HandleAsync(Command(product.Id), CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(CatalogErrorCodes.ProductSkuTaken);
    }

    [Fact]
    public async Task Omitting_the_deposit_makes_the_product_non_returnable()
    {
        var (handler, repository, _) = Build();
        var product = repository.Seed(Existing());
        product.IsReturnable.ShouldBeTrue();

        var result = await handler.HandleAsync(Command(product.Id, deposit: null), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        product.IsReturnable.ShouldBeFalse();
        product.DepositPerBaseUnit.ShouldBeNull();
    }

    [Fact]
    public async Task Packagings_left_out_are_removed_rather_than_kept()
    {
        var (handler, repository, _) = Build();
        var product = repository.Seed(Existing());

        var result = await handler.HandleAsync(Command(product.Id, packagings: []), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        product.Packagings.ShouldBeEmpty();
    }

    [Fact]
    public async Task An_existing_packaging_is_repriced_in_place()
    {
        var (handler, repository, _) = Build();
        var product = repository.Seed(Existing());

        var result = await handler.HandleAsync(Command(product.Id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        var crate = product.FindPackaging("CRATE").ShouldNotBeNull();
        crate.SellingPrice.Amount.ShouldBe(21000m);
    }
}
