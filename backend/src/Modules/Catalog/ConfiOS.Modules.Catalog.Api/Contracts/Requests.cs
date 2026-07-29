namespace ConfiOS.Modules.Catalog.Api.Contracts;

/// <summary>Payload for adding a product.</summary>
/// <param name="Name">Display name.</param>
/// <param name="Sku">Stock-keeping unit, unique within the business.</param>
/// <param name="PriceAmount">Price amount in the given currency.</param>
/// <param name="CurrencyCode">ISO 4217 currency code.</param>
public sealed record CreateProductRequest(
    string Name,
    string Sku,
    decimal PriceAmount,
    string CurrencyCode);
