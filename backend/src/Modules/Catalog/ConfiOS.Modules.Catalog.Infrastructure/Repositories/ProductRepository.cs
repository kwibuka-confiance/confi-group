using ConfiOS.Modules.Catalog.Application.Abstractions;
using ConfiOS.Modules.Catalog.Domain.Products;
using ConfiOS.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConfiOS.Modules.Catalog.Infrastructure.Repositories;

/// <summary>
/// Product storage. Every query runs through the tenant filter applied in
/// <c>TenantDbContext</c>, so these methods only ever see the current tenant's products.
/// </summary>
/// <param name="context">Catalog database context.</param>
public sealed class ProductRepository(CatalogDbContext context) : IProductRepository
{
    public Task<bool> SkuExistsAsync(
        string sku,
        Guid? excludingProductId = null,
        CancellationToken cancellationToken = default) =>
        context.Products.AnyAsync(
            product => product.Sku == sku
                && (excludingProductId == null || product.Id != excludingProductId),
            cancellationToken);

    public Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.Products
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public void Add(Product product) => context.Products.Add(product);
}
