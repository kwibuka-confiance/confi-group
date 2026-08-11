namespace ConfiOS.Modules.Catalog.Api.Contracts;

/// <summary>
/// Payload for adding a product.
/// </summary>
/// <remarks>
/// Every amount is in <paramref name="CurrencyCode"/>. Only the name, SKU, price
/// and currency are required; a product sold as individual items with no larger
/// unit needs nothing else.
/// </remarks>
/// <param name="Name">Display name.</param>
/// <param name="Sku">Stock-keeping unit, unique within the business.</param>
/// <param name="PriceAmount">Selling price of one base unit.</param>
/// <param name="CurrencyCode">ISO 4217 currency code for every amount here.</param>
/// <param name="BaseUnitCode">
/// Unit stock is counted in, for example <c>BOTTLE</c>. Defaults to individual items.
/// </param>
/// <param name="Description">Optional longer description.</param>
/// <param name="CostAmount">What one base unit costs the business, when known.</param>
/// <param name="TaxClass">One of <c>Standard</c>, <c>Zero</c> or <c>Exempt</c>.</param>
/// <param name="DepositAmount">
/// Deposit held per base unit. Supplying it marks the product returnable.
/// </param>
/// <param name="Packagings">Larger units the product is counted and sold in.</param>
public sealed record CreateProductRequest(
    string Name,
    string Sku,
    decimal PriceAmount,
    string CurrencyCode,
    string? BaseUnitCode = null,
    string? Description = null,
    decimal? CostAmount = null,
    string? TaxClass = null,
    decimal? DepositAmount = null,
    IReadOnlyList<CreatePackagingRequest>? Packagings = null);

/// <summary>A way of counting the product, supplied when it is created.</summary>
/// <param name="UnitCode">Unit of measure code, for example <c>CRATE</c>.</param>
/// <param name="QuantityInBaseUnit">How many base units it holds, for example 24.</param>
/// <param name="SellingPriceAmount">Price of one of these.</param>
/// <param name="CostAmount">What one costs the business, when known.</param>
/// <param name="Barcode">Scanned code for this packaging.</param>
public sealed record CreatePackagingRequest(
    string UnitCode,
    int QuantityInBaseUnit,
    decimal SellingPriceAmount,
    decimal? CostAmount = null,
    string? Barcode = null);
