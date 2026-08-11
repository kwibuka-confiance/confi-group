using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Domain;
using ConfiOS.Modules.Catalog.Domain.Products;

namespace ConfiOS.Modules.Catalog.Application.Products.CreateProduct;

/// <summary>Creates a product, rejecting a SKU already used within the business.</summary>
/// <param name="products">Product repository (already tenant-scoped).</param>
/// <param name="tenantContext">The resolved tenant.</param>
/// <param name="unitOfWork">Commits the change.</param>
public sealed class CreateProductHandler(
    IProductRepository products,
    ITenantContext tenantContext,
    ICatalogUnitOfWork unitOfWork) : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var sku = command.Sku.Trim().ToUpperInvariant();

        if (await products.SkuExistsAsync(sku, cancellationToken: cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<Guid>(Error.Conflict(CatalogErrorCodes.ProductSkuTaken));
        }

        var currency = Currency.FromCode(command.CurrencyCode);
        Money Amount(decimal value) => Money.Of(value, currency);

        var product = Product.Create(
            tenantContext.TenantId,
            command.Name,
            sku,
            Amount(command.PriceAmount),
            command.BaseUnitCode,
            command.Description);

        if (command.CostAmount is { } cost)
        {
            product.SetCostPrice(Amount(cost));
        }

        if (TryParseTaxClass(command.TaxClass, out var taxClass))
        {
            product.SetTaxClass(taxClass);
        }

        // A deposit is what marks a base unit as returnable: it is the amount that
        // settles the exchange when a customer takes more than they bring back.
        if (command.DepositAmount is { } deposit)
        {
            product.MakeReturnable(Amount(deposit));
        }

        foreach (var packaging in command.Packagings ?? [])
        {
            product.AddPackaging(Packaging.Create(
                packaging.UnitCode,
                packaging.QuantityInBaseUnit,
                Amount(packaging.SellingPriceAmount),
                packaging.CostAmount is { } packagingCost ? Amount(packagingCost) : null,
                packaging.Barcode));
        }

        products.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(product.Id);
    }

    /// <summary>Unrecognised values fall back to the standard rate rather than failing.</summary>
    private static bool TryParseTaxClass(string? value, out TaxClass taxClass)
    {
        taxClass = TaxClass.Standard;
        return !string.IsNullOrWhiteSpace(value)
            && Enum.TryParse(value, ignoreCase: true, out taxClass);
    }
}
