using ConfiOS.BuildingBlocks.Application.Messaging;

namespace ConfiOS.Modules.Catalog.Application.Products.GetProducts;

/// <summary>Lists the current tenant's products.</summary>
public sealed record GetProductsQuery : IQuery<IReadOnlyList<ProductSummary>>;

/// <summary>A product as shown in a list.</summary>
/// <param name="Id">Product identifier.</param>
/// <param name="Name">Display name.</param>
/// <param name="Sku">Stock-keeping unit.</param>
/// <param name="PriceAmount">Price amount.</param>
/// <param name="CurrencyCode">ISO 4217 currency code.</param>
/// <param name="IsActive">Whether the product is currently sold.</param>
public sealed record ProductSummary(
    Guid Id,
    string Name,
    string Sku,
    decimal PriceAmount,
    string CurrencyCode,
    bool IsActive);
