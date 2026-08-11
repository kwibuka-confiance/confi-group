using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Application.Products.GetProducts;
using ConfiOS.Modules.Catalog.Domain;

namespace ConfiOS.Modules.Catalog.Application.Products.GetProduct;

/// <summary>
/// Returns one product, so an edit form can be filled with what is stored rather
/// than with whatever the list happened to be showing.
/// </summary>
/// <param name="products">Product repository (already tenant-scoped).</param>
public sealed class GetProductHandler(IProductRepository products)
    : IQueryHandler<GetProductQuery, ProductSummary>
{
    public async Task<Result<ProductSummary>> HandleAsync(
        GetProductQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var product = await products.GetAsync(query.ProductId, cancellationToken).ConfigureAwait(false);

        // Another tenant's product is filtered out before it gets here, so it is
        // reported as missing rather than forbidden: the caller learns nothing about
        // whether it exists elsewhere.
        return product is null
            ? Result.Failure<ProductSummary>(Error.NotFound(CatalogErrorCodes.ProductNotFound))
            : Result.Success(ProductSummary.From(product));
    }
}
