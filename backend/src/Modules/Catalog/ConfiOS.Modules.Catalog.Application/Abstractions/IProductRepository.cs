using ConfiOS.Modules.Catalog.Domain.Products;

namespace ConfiOS.Modules.Catalog.Application.Abstractions;

/// <summary>Loads and stores products within the resolved tenant.</summary>
public interface IProductRepository
{
    /// <summary>
    /// Whether the SKU is already used. <paramref name="excludingProductId"/> lets a
    /// product keep its own SKU when edited without colliding with itself.
    /// </summary>
    Task<bool> SkuExistsAsync(
        string sku,
        Guid? excludingProductId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Loads one product with its packagings, or null when it is not this tenant's.</summary>
    Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default);

    void Add(Product product);
}
