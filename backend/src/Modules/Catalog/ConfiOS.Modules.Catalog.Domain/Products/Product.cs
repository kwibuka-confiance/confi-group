using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;

namespace ConfiOS.Modules.Catalog.Domain.Products;

/// <summary>
/// Something a business sells. Industry-agnostic by design: it carries a name, a SKU unique
/// within the business, and a price, and nothing specific to any trade.
/// </summary>
public sealed class Product : TenantEntity
{
    private Product(Guid id, TenantId tenantId, string name, string sku, Money price)
        : base(id, tenantId)
    {
        Name = name;
        Sku = sku;
        Price = price;
        IsActive = true;
    }

    private Product()
    {
    }

    public string Name { get; private set; } = string.Empty;

    /// <summary>Stock-keeping unit, unique within the business and stored upper-cased.</summary>
    public string Sku { get; private set; } = string.Empty;

    public Money Price { get; private set; } = null!;

    /// <summary>Whether the product is sold. Archived products are kept for history.</summary>
    public bool IsActive { get; private set; }

    public static Product Create(TenantId tenantId, string name, string sku, Money price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentNullException.ThrowIfNull(price);
        EnsureNonNegative(price);

        return new Product(
            Guid.CreateVersion7(),
            tenantId,
            name.Trim(),
            sku.Trim().ToUpperInvariant(),
            price);
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public void Reprice(Money price)
    {
        ArgumentNullException.ThrowIfNull(price);
        EnsureNonNegative(price);
        Price = price;
    }

    public void Archive() => IsActive = false;

    public void Activate() => IsActive = true;

    private static void EnsureNonNegative(Money price)
    {
        if (price.IsNegative)
        {
            throw new DomainException(Error.Validation(ErrorCodes.NegativeAmount));
        }
    }
}
