using ConfiOS.BuildingBlocks.Application.Messaging;

namespace ConfiOS.Modules.Catalog.Application.Products.CreateProduct;

/// <summary>Adds a product to the current tenant's catalog.</summary>
/// <param name="Name">Display name.</param>
/// <param name="Sku">Stock-keeping unit, unique within the business.</param>
/// <param name="PriceAmount">Price amount in the given currency.</param>
/// <param name="CurrencyCode">ISO 4217 currency code.</param>
public sealed record CreateProductCommand(
    string Name,
    string Sku,
    decimal PriceAmount,
    string CurrencyCode) : ICommand<Guid>;
