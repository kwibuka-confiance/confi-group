using ConfiOS.BuildingBlocks.Application.Auditing;
using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Domain;
using ConfiOS.Modules.Catalog.Domain.Products;

namespace ConfiOS.Modules.Catalog.Application.Products.UpdateProduct;

/// <summary>
/// Corrects a product. What is stored is replaced by what is sent, so an omitted
/// optional field is cleared rather than left behind.
/// </summary>
/// <remarks>
/// Prices are what a business gets wrong most often and what customers are charged
/// from, so a change is written to the audit log with its before and after values.
/// </remarks>
/// <param name="products">Product repository (already tenant-scoped).</param>
/// <param name="tenantContext">The resolved tenant, branch and user.</param>
/// <param name="auditLogger">Records the change alongside it, in one transaction.</param>
/// <param name="unitOfWork">Commits the change.</param>
public sealed class UpdateProductHandler(
    IProductRepository products,
    ITenantContext tenantContext,
    ICatalogAuditLogger auditLogger,
    ICatalogUnitOfWork unitOfWork) : ICommandHandler<UpdateProductCommand>
{
    public async Task<Result> HandleAsync(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var product = await products.GetAsync(command.ProductId, cancellationToken).ConfigureAwait(false);
        if (product is null)
        {
            return Result.Failure(Error.NotFound(CatalogErrorCodes.ProductNotFound));
        }

        var sku = command.Sku.Trim().ToUpperInvariant();

        // Excluding this product lets it keep its own SKU; any other holder is a clash.
        if (await products.SkuExistsAsync(sku, product.Id, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure(Error.Conflict(CatalogErrorCodes.ProductSkuTaken));
        }

        var previousPrice = product.Price.Amount;
        var previousSku = product.Sku;

        var currency = Currency.FromCode(command.CurrencyCode);
        Money Amount(decimal value) => Money.Of(value, currency);

        product.Rename(command.Name);
        product.SetSku(sku);
        product.Describe(command.Description);
        product.Reprice(Amount(command.PriceAmount));
        product.SetCostPrice(command.CostAmount is { } cost ? Amount(cost) : null);
        product.SetTaxClass(ParseTaxClass(command.TaxClass));

        if (command.DepositAmount is { } deposit)
        {
            product.MakeReturnable(Amount(deposit));
        }
        else
        {
            product.MakeNonReturnable();
        }

        product.ReplacePackagings(
            (command.Packagings ?? []).Select(packaging => Packaging.Create(
                packaging.UnitCode,
                packaging.QuantityInBaseUnit,
                Amount(packaging.SellingPriceAmount),
                packaging.CostAmount is { } packagingCost ? Amount(packagingCost) : null,
                packaging.Barcode)));

        await auditLogger.RecordAsync(
            new AuditEntry(
                "catalog.product-updated",
                nameof(Product),
                product.Id,
                tenantContext.TenantId,
                tenantContext.BranchId,
                tenantContext.UserId,
                new Dictionary<string, object?>
                {
                    ["skuBefore"] = previousSku,
                    ["skuAfter"] = product.Sku,
                    ["priceBefore"] = previousPrice,
                    ["priceAfter"] = product.Price.Amount,
                    ["currency"] = currency.Code,
                }),
            cancellationToken).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }

    /// <summary>Unrecognised values fall back to the standard rate rather than failing.</summary>
    private static TaxClass ParseTaxClass(string? value) =>
        !string.IsNullOrWhiteSpace(value) && Enum.TryParse<TaxClass>(value, ignoreCase: true, out var taxClass)
            ? taxClass
            : TaxClass.Standard;
}
