using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Catalog.Application.Products.SetProductStatus;
using ConfiOS.Modules.Catalog.Domain;
using ConfiOS.Modules.Catalog.Domain.Products;
using Shouldly;
using Xunit;

namespace ConfiOS.Modules.Catalog.Tests.Products;

public sealed class SetProductStatusHandlerTests
{
    private static Product Existing() => Product.Create(
        TenantId.New(),
        "Mutsig Small 330ml",
        "MUT-330",
        Money.Of(800, Currency.FromCode("RWF")),
        "BOTTLE");

    private static (SetProductStatusHandler Handler, FakeProductRepository Repository, FakeAuditLogger Audit, FakeUnitOfWork Work)
        Build()
    {
        var repository = new FakeProductRepository();
        var audit = new FakeAuditLogger();
        var work = new FakeUnitOfWork();
        return (new SetProductStatusHandler(repository, new FakeTenantContext(), audit, work), repository, audit, work);
    }

    [Fact]
    public async Task Archiving_withdraws_the_product_and_records_who_did_it()
    {
        var (handler, repository, audit, _) = Build();
        var product = repository.Seed(Existing());

        var result = await handler.HandleAsync(new SetProductStatusCommand(product.Id, false), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        product.IsActive.ShouldBeFalse();
        audit.Entries.ShouldHaveSingleItem().Action.ShouldBe("catalog.product-archived");
    }

    [Fact]
    public async Task Restoring_puts_it_back_on_sale()
    {
        var (handler, repository, audit, _) = Build();
        var product = repository.Seed(Existing());
        product.Archive();

        var result = await handler.HandleAsync(new SetProductStatusCommand(product.Id, true), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        product.IsActive.ShouldBeTrue();
        audit.Entries.ShouldHaveSingleItem().Action.ShouldBe("catalog.product-restored");
    }

    [Fact]
    public async Task Archiving_an_archived_product_changes_nothing_and_is_not_audited()
    {
        var (handler, repository, audit, work) = Build();
        var product = repository.Seed(Existing());
        product.Archive();

        var result = await handler.HandleAsync(new SetProductStatusCommand(product.Id, false), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        audit.Entries.ShouldBeEmpty();
        work.SaveCount.ShouldBe(0);
    }

    [Fact]
    public async Task A_missing_product_is_reported_as_not_found()
    {
        var (handler, _, _, _) = Build();

        var result = await handler.HandleAsync(
            new SetProductStatusCommand(Guid.CreateVersion7(), false),
            CancellationToken.None);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe(CatalogErrorCodes.ProductNotFound);
    }
}
