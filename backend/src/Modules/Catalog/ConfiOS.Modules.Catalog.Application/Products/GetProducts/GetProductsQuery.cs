using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.Modules.Catalog.Domain.Products;

namespace ConfiOS.Modules.Catalog.Application.Products.GetProducts;

/// <summary>Lists the current tenant's products.</summary>
public sealed record GetProductsQuery : IQuery<IReadOnlyList<ProductSummary>>;

/// <summary>A product as shown in a list.</summary>
/// <param name="Id">Product identifier.</param>
/// <param name="Name">Display name.</param>
/// <param name="Sku">Stock-keeping unit.</param>
/// <param name="Description">Longer description, when set.</param>
/// <param name="PriceAmount">Selling price of one base unit.</param>
/// <param name="CostAmount">Cost of one base unit, when known.</param>
/// <param name="CurrencyCode">ISO 4217 currency code.</param>
/// <param name="BaseUnitCode">Unit stock is counted in.</param>
/// <param name="TaxClass">How the product is taxed.</param>
/// <param name="IsReturnable">Whether the base unit is exchanged rather than sold outright.</param>
/// <param name="DepositAmount">Deposit per base unit, when returnable.</param>
/// <param name="Packagings">Larger units it is counted and sold in.</param>
/// <param name="IsActive">Whether the product is currently sold.</param>
public sealed record ProductSummary(
    Guid Id,
    string Name,
    string Sku,
    string? Description,
    decimal PriceAmount,
    decimal? CostAmount,
    string CurrencyCode,
    string BaseUnitCode,
    string TaxClass,
    bool IsReturnable,
    decimal? DepositAmount,
    IReadOnlyList<PackagingSummary> Packagings,
    bool IsActive)
{
    /// <summary>
    /// Projects a product for the wire. Shared by the list and the single-product
    /// read so the two can never drift into describing a product differently.
    /// </summary>
    public static ProductSummary From(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        return new ProductSummary(
            product.Id,
            product.Name,
            product.Sku,
            product.Description,
            product.Price.Amount,
            product.CostPrice?.Amount,
            product.Price.Currency.Code,
            product.BaseUnitCode,
            product.TaxClass.ToString(),
            product.IsReturnable,
            product.DepositPerBaseUnit?.Amount,
            product.Packagings
                .OrderBy(packaging => packaging.QuantityInBaseUnit)
                .Select(packaging => new PackagingSummary(
                    packaging.UnitCode,
                    packaging.QuantityInBaseUnit,
                    packaging.SellingPrice.Amount,
                    packaging.CostPrice?.Amount,
                    packaging.Barcode,
                    packaging.AllowsQuarters))
                .ToList(),
            product.IsActive);
    }
}

/// <summary>A packaging as shown alongside its product.</summary>
/// <param name="UnitCode">Unit of measure code, for example <c>CRATE</c>.</param>
/// <param name="QuantityInBaseUnit">How many base units it holds.</param>
/// <param name="SellingPriceAmount">Price of one of these.</param>
/// <param name="CostAmount">What one costs the business, when known.</param>
/// <param name="Barcode">Scanned code for this packaging.</param>
/// <param name="AllowsQuarters">
/// Whether it can be sold in quarters. False when a quarter would not come to a
/// whole base unit, so the client can hide the option rather than offer a sale
/// that will be rejected.
/// </param>
public sealed record PackagingSummary(
    string UnitCode,
    int QuantityInBaseUnit,
    decimal SellingPriceAmount,
    decimal? CostAmount,
    string? Barcode,
    bool AllowsQuarters);
