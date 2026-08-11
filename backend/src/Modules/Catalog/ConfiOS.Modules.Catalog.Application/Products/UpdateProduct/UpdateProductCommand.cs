using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.Modules.Catalog.Application.Products.CreateProduct;

namespace ConfiOS.Modules.Catalog.Application.Products.UpdateProduct;

/// <summary>
/// Corrects a product in the current tenant's catalog.
/// </summary>
/// <remarks>
/// This is a whole-record replacement, not a patch: every field is stated and the
/// packagings given become the complete set, so one left out is one the business no
/// longer sells. An omitted optional field is therefore a instruction to clear it.
/// <para>
/// The base unit cannot be changed here. Stock is counted in it and every packaging
/// is a multiple of it, so altering it would silently reinterpret quantities the
/// business has already recorded.
/// </para>
/// </remarks>
/// <param name="ProductId">Which product to correct.</param>
/// <param name="Name">Display name.</param>
/// <param name="Sku">Stock-keeping unit, unique within the business.</param>
/// <param name="PriceAmount">Selling price of one base unit.</param>
/// <param name="CurrencyCode">ISO 4217 currency code for every amount here.</param>
/// <param name="Description">Longer description, or null to clear it.</param>
/// <param name="CostAmount">Cost of one base unit, or null to clear it.</param>
/// <param name="TaxClass">How the product is taxed. Defaults to the standard rate.</param>
/// <param name="DepositAmount">
/// Deposit per base unit. Supplying it marks the product returnable; null makes it
/// non-returnable again.
/// </param>
/// <param name="Packagings">The complete set of larger units it is sold in.</param>
public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Sku,
    decimal PriceAmount,
    string CurrencyCode,
    string? Description = null,
    decimal? CostAmount = null,
    string? TaxClass = null,
    decimal? DepositAmount = null,
    IReadOnlyList<PackagingInput>? Packagings = null) : ICommand;
