using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.Modules.Catalog.Application.Abstractions;

namespace ConfiOS.Modules.Catalog.Application.Products.GetProducts;

/// <summary>Returns the tenant's products, most usefully ordered by name.</summary>
/// <param name="products">Product repository (already tenant-scoped).</param>
public sealed class GetProductsHandler(IProductRepository products)
    : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductSummary>>
{
    public async Task<Result<IReadOnlyList<ProductSummary>>> HandleAsync(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        var items = await products.ListAsync(cancellationToken).ConfigureAwait(false);

        IReadOnlyList<ProductSummary> summaries = items
            .Select(product => new ProductSummary(
                product.Id,
                product.Name,
                product.Sku,
                product.Price.Amount,
                product.Price.Currency.Code,
                product.IsActive))
            .ToList();

        return Result.Success(summaries);
    }
}
