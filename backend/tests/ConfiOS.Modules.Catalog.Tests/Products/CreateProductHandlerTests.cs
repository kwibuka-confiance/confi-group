using ConfiOS.BuildingBlocks.Application.Abstractions;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Application.Products.CreateProduct;
using ConfiOS.Modules.Catalog.Domain;
using ConfiOS.Modules.Catalog.Domain.Products;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Catalog.Tests.Products;

public sealed class CreateProductHandlerTests
{
    private static CreateProductCommand MutsigSmall() => new(
        "Mutsig Small 330ml",
        "MUT-330",
        PriceAmount: 800,
        CurrencyCode: "RWF",
        BaseUnitCode: "BOTTLE",
        Description: "Returnable 330ml bottle",
        CostAmount: 600,
        TaxClass: "Standard",
        DepositAmount: 100,
        Packagings: [new PackagingInput("CRATE", 24, 18000, 15000, "5901234123457")]);

    [Fact]
    public async Task A_product_is_created_with_its_packaging()
    {
        var repository = new FakeProductRepository();
        var handler = CreateHandler(repository);

        var result = await handler.HandleAsync(MutsigSmall(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        var product = repository.Added.ShouldHaveSingleItem();
        product.Sku.ShouldBe("MUT-330");
        product.BaseUnitCode.ShouldBe("BOTTLE");
        product.CostPrice!.Amount.ShouldBe(600m);
        product.TaxClass.ShouldBe(TaxClass.Standard);

        var crate = product.FindPackaging("CRATE").ShouldNotBeNull();
        crate.QuantityInBaseUnit.ShouldBe(24);
        crate.SellingPrice.Amount.ShouldBe(18000m);
        crate.Barcode.ShouldBe("5901234123457");
        crate.ToBaseUnits(0.25m).ShouldBe(6);
    }

    [Fact]
    public async Task A_deposit_marks_the_product_returnable()
    {
        var repository = new FakeProductRepository();
        var handler = CreateHandler(repository);

        await handler.HandleAsync(MutsigSmall(), CancellationToken.None);

        var product = repository.Added.ShouldHaveSingleItem();
        product.IsReturnable.ShouldBeTrue();
        product.DepositPerBaseUnit!.Amount.ShouldBe(100m);
    }

    [Fact]
    public async Task A_product_without_extras_still_creates()
    {
        var repository = new FakeProductRepository();
        var handler = CreateHandler(repository);

        var result = await handler.HandleAsync(
            new CreateProductCommand("Bread", "BRD-1", 1200, "RWF"),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        var product = repository.Added.ShouldHaveSingleItem();
        product.BaseUnitCode.ShouldBe(Product.DefaultBaseUnit);
        product.IsReturnable.ShouldBeFalse();
        product.CostPrice.ShouldBeNull();
        product.Packagings.ShouldBeEmpty();
    }

    [Fact]
    public async Task A_duplicate_sku_is_rejected()
    {
        var repository = new FakeProductRepository { SkuTaken = true };
        var handler = CreateHandler(repository);

        var result = await handler.HandleAsync(MutsigSmall(), CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(CatalogErrorCodes.ProductSkuTaken);
        repository.Added.ShouldBeEmpty();
    }

    private static CreateProductHandler CreateHandler(FakeProductRepository repository) =>
        new(repository, new FakeTenantContext(), new FakeUnitOfWork());

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Added { get; } = [];

        public bool SkuTaken { get; init; }

        public Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default) =>
            Task.FromResult(SkuTaken);

        public Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Product>>(Added);

        public void Add(Product product) => Added.Add(product);
    }

    private sealed class FakeTenantContext : ITenantContext
    {
        public bool IsResolved => true;

        public TenantId TenantId { get; } = TenantId.New();

        public BranchId? BranchId => null;

        public UserId? UserId => null;

        public string Locale => "en";

        public string CurrencyCode => "RWF";

        public string TimeZoneId => "Africa/Kigali";
    }

    private sealed class FakeUnitOfWork : ICatalogUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(1);

        public Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IAsyncDisposable>(new NoopTransaction());

        private sealed class NoopTransaction : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}
