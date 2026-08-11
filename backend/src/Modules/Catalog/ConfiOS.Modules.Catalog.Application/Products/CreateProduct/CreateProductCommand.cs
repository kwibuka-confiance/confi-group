using ConfiOS.BuildingBlocks.Application.Messaging;

namespace ConfiOS.Modules.Catalog.Application.Products.CreateProduct;

/// <summary>
/// Adds a product to the current tenant's catalog.
/// </summary>
/// <remarks>
/// Every amount is in <paramref name="CurrencyCode"/>: a product's prices are all
/// quoted in one currency, so the caller states it once.
/// </remarks>
/// <param name="Name">Display name.</param>
/// <param name="Sku">Stock-keeping unit, unique within the business.</param>
/// <param name="PriceAmount">Selling price of one base unit.</param>
/// <param name="CurrencyCode">ISO 4217 currency code for every amount here.</param>
/// <param name="BaseUnitCode">Unit stock is counted in. Defaults to individual items.</param>
/// <param name="Description">Optional longer description.</param>
/// <param name="CostAmount">What one base unit costs the business, when known.</param>
/// <param name="TaxClass">How the product is taxed. Defaults to the standard rate.</param>
/// <param name="DepositAmount">
/// Deposit held per base unit. Supplying it marks the product returnable, which is
/// how bottles exchanged with customers are modelled.
/// </param>
/// <param name="Packagings">Larger units the product is counted and sold in.</param>
public sealed record CreateProductCommand(
    string Name,
    string Sku,
    decimal PriceAmount,
    string CurrencyCode,
    string? BaseUnitCode = null,
    string? Description = null,
    decimal? CostAmount = null,
    string? TaxClass = null,
    decimal? DepositAmount = null,
    IReadOnlyList<PackagingInput>? Packagings = null) : ICommand<Guid>;

/// <summary>A way of counting the product, supplied when it is created.</summary>
/// <param name="UnitCode">Unit of measure code, for example <c>CRATE</c>.</param>
/// <param name="QuantityInBaseUnit">How many base units it holds.</param>
/// <param name="SellingPriceAmount">Price of one of these.</param>
/// <param name="CostAmount">What one costs the business, when known.</param>
/// <param name="Barcode">Scanned code for this packaging.</param>
public sealed record PackagingInput(
    string UnitCode,
    int QuantityInBaseUnit,
    decimal SellingPriceAmount,
    decimal? CostAmount = null,
    string? Barcode = null);
