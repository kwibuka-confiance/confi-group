using ConfiOS.Modules.Catalog.Domain.Products;

namespace ConfiOS.Modules.Catalog.Application.Abstractions;

/// <summary>Loads and stores products within the resolved tenant.</summary>
public interface IProductRepository
{
    Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default);

    void Add(Product product);
}
