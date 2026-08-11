using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.Modules.Catalog.Application.Products.GetProducts;

namespace ConfiOS.Modules.Catalog.Application.Products.GetProduct;

/// <summary>Reads one of the current tenant's products.</summary>
/// <param name="ProductId">Identifier of the product to read.</param>
public sealed record GetProductQuery(Guid ProductId) : IQuery<ProductSummary>;
