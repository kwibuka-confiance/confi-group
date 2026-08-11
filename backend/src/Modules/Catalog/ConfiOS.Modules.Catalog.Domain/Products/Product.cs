using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.Primitives;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;

namespace ConfiOS.Modules.Catalog.Domain.Products;

/// <summary>
/// Something a business sells. Industry-agnostic by design: it carries a name, a
/// SKU unique within the business, a price and a unit, and nothing specific to any
/// trade.
/// </summary>
/// <remarks>
/// Quantities of a product are held in its <see cref="BaseUnitCode"/> — the
/// indivisible physical thing, such as a bottle. Other ways of counting it, like a
/// crate of 24, are <see cref="Packagings"/> that convert to the base unit rather
/// than separate products, so stock can never disagree with itself (ADR-010).
/// </remarks>
public sealed class Product : TenantEntity
{
    /// <summary>Used when a product is sold as individual items with no larger unit.</summary>
    public const string DefaultBaseUnit = "EA";

    private readonly List<Packaging> _packagings = [];

    private Product(
        Guid id,
        TenantId tenantId,
        string name,
        string sku,
        Money price,
        string baseUnitCode)
        : base(id, tenantId)
    {
        Name = name;
        Sku = sku;
        Price = price;
        BaseUnitCode = baseUnitCode;
        TaxClass = TaxClass.Standard;
        IsActive = true;
    }

    private Product()
    {
    }

    public string Name { get; private set; } = string.Empty;

    /// <summary>Stock-keeping unit, unique within the business and stored upper-cased.</summary>
    public string Sku { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    /// <summary>Selling price of one base unit.</summary>
    public Money Price { get; private set; } = null!;

    /// <summary>
    /// What one base unit costs the business. Absent until known; without it no
    /// margin can be reported.
    /// </summary>
    public Money? CostPrice { get; private set; }

    /// <summary>Unit stock is counted in, for example <c>BOTTLE</c>.</summary>
    public string BaseUnitCode { get; private set; } = DefaultBaseUnit;

    /// <summary>How the product is taxed. Drives the rate applied when invoicing.</summary>
    public TaxClass TaxClass { get; private set; }

    /// <summary>
    /// Whether the base unit is returned and refilled rather than consumed, as a
    /// bottle is. Returnables are exchanged with the customer and settled against a
    /// balance rather than sold outright.
    /// </summary>
    public bool IsReturnable { get; private set; }

    /// <summary>Held per returnable base unit and repayable, so never revenue.</summary>
    public Money? DepositPerBaseUnit { get; private set; }

    /// <summary>Larger units this product is counted and sold in.</summary>
    public IReadOnlyCollection<Packaging> Packagings => _packagings.AsReadOnly();

    /// <summary>Whether the product is sold. Archived products are kept for history.</summary>
    public bool IsActive { get; private set; }

    public static Product Create(
        TenantId tenantId,
        string name,
        string sku,
        Money price,
        string? baseUnitCode = null,
        string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentNullException.ThrowIfNull(price);
        EnsureNonNegative(price);

        var product = new Product(
            Guid.CreateVersion7(),
            tenantId,
            name.Trim(),
            sku.Trim().ToUpperInvariant(),
            price,
            string.IsNullOrWhiteSpace(baseUnitCode)
                ? DefaultBaseUnit
                : baseUnitCode.Trim().ToUpperInvariant());

        product.Describe(description);
        return product;
    }

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    /// <summary>
    /// Changes the stock-keeping unit. Uniqueness within the business is enforced by
    /// the caller, which is the only place that can see the other products.
    /// </summary>
    public void SetSku(string sku)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        Sku = sku.Trim().ToUpperInvariant();
    }

    public void Describe(string? description) =>
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

    public void Reprice(Money price)
    {
        ArgumentNullException.ThrowIfNull(price);
        EnsureNonNegative(price);
        Price = price;
    }

    public void SetCostPrice(Money? costPrice)
    {
        if (costPrice is not null)
        {
            EnsureNonNegative(costPrice);
        }

        CostPrice = costPrice;
    }

    public void SetTaxClass(TaxClass taxClass) => TaxClass = taxClass;

    /// <summary>
    /// Marks the base unit as returnable and sets the deposit that settles an
    /// imbalance when a customer takes more than they bring back.
    /// </summary>
    public void MakeReturnable(Money depositPerBaseUnit)
    {
        ArgumentNullException.ThrowIfNull(depositPerBaseUnit);
        EnsureNonNegative(depositPerBaseUnit);

        IsReturnable = true;
        DepositPerBaseUnit = depositPerBaseUnit;
    }

    public void MakeNonReturnable()
    {
        IsReturnable = false;
        DepositPerBaseUnit = null;
    }

    /// <summary>
    /// Adds a way of counting this product. A unit may only be defined once, so a
    /// product cannot have two conflicting ideas of what a crate holds.
    /// </summary>
    public void AddPackaging(Packaging packaging)
    {
        ArgumentNullException.ThrowIfNull(packaging);

        if (_packagings.Exists(existing =>
            string.Equals(existing.UnitCode, packaging.UnitCode, StringComparison.Ordinal)))
        {
            throw new DomainException(Error.Conflict(CatalogErrorCodes.PackagingUnitTaken));
        }

        if (string.Equals(packaging.UnitCode, BaseUnitCode, StringComparison.Ordinal))
        {
            throw new DomainException(Error.Validation(CatalogErrorCodes.PackagingIsBaseUnit));
        }

        _packagings.Add(packaging);
    }

    /// <summary>
    /// Replaces every way of counting this product with the ones given.
    /// </summary>
    /// <remarks>
    /// Editing a product submits the full set rather than a patch, so a packaging
    /// left out is one the business no longer sells. Replacing wholesale keeps that
    /// unambiguous, and each entry is still validated, so the result cannot hold two
    /// definitions of a crate.
    /// </remarks>
    public void ReplacePackagings(IEnumerable<Packaging> packagings)
    {
        ArgumentNullException.ThrowIfNull(packagings);

        // Validated in full before anything is discarded, so a rejected entry leaves
        // the product exactly as it was rather than half-replaced.
        var replacements = new List<Packaging>();
        foreach (var packaging in packagings)
        {
            ArgumentNullException.ThrowIfNull(packaging);

            if (replacements.Exists(existing =>
                string.Equals(existing.UnitCode, packaging.UnitCode, StringComparison.Ordinal)))
            {
                throw new DomainException(Error.Conflict(CatalogErrorCodes.PackagingUnitTaken));
            }

            if (string.Equals(packaging.UnitCode, BaseUnitCode, StringComparison.Ordinal))
            {
                throw new DomainException(Error.Validation(CatalogErrorCodes.PackagingIsBaseUnit));
            }

            replacements.Add(packaging);
        }

        _packagings.Clear();
        _packagings.AddRange(replacements);
    }

    public void RemovePackaging(string unitCode) =>
        _packagings.RemoveAll(packaging =>
            string.Equals(packaging.UnitCode, unitCode.Trim().ToUpperInvariant(), StringComparison.Ordinal));

    /// <summary>Finds a packaging by unit code, or null when the product has none.</summary>
    public Packaging? FindPackaging(string unitCode) =>
        _packagings.Find(packaging =>
            string.Equals(packaging.UnitCode, unitCode.Trim().ToUpperInvariant(), StringComparison.Ordinal));

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

/// <summary>How a product is taxed. The rate itself is a tenant setting.</summary>
public enum TaxClass
{
    /// <summary>The tenant's standard rate applies.</summary>
    Standard = 0,

    /// <summary>Taxable, but at zero percent.</summary>
    Zero = 1,

    /// <summary>Outside the tax system entirely.</summary>
    Exempt = 2,
}
