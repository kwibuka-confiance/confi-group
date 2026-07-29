using ConfiOS.BuildingBlocks.Application.Context;
using ConfiOS.BuildingBlocks.Application.Messaging;
using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;
using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Domain;
using ConfiOS.Modules.Catalog.Domain.Products;

namespace ConfiOS.Modules.Catalog.Application.Products.CreateProduct;

/// <summary>Creates a product, rejecting a SKU already used within the business.</summary>
/// <param name="products">Product repository (already tenant-scoped).</param>
/// <param name="tenantContext">The resolved tenant.</param>
/// <param name="unitOfWork">Commits the change.</param>
public sealed class CreateProductHandler(
    IProductRepository products,
    ITenantContext tenantContext,
    ICatalogUnitOfWork unitOfWork) : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var sku = command.Sku.Trim().ToUpperInvariant();

        if (await products.SkuExistsAsync(sku, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<Guid>(Error.Conflict(CatalogErrorCodes.ProductSkuTaken));
        }

        var price = Money.Of(command.PriceAmount, command.CurrencyCode);
        var product = Product.Create(tenantContext.TenantId, command.Name, sku, price);

        products.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success(product.Id);
    }
}
