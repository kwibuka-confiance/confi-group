using ConfiOS.BuildingBlocks.Domain.Errors;
using ConfiOS.BuildingBlocks.Domain.ValueObjects;

namespace ConfiOS.Modules.Catalog.Domain.Products;

/// <summary>
/// A way of counting and selling a product, expressed as a multiple of the
/// product's base unit — a crate of 24 bottles, a box of 100 tablets.
/// </summary>
/// <remarks>
/// A packaging is a unit of measure, not a container that changes hands. Stock is
/// always held in the base unit, so selling one crate of 24 removes 24 bottles and
/// the two figures can never disagree (ADR-010).
/// <para>
/// Each packaging carries its own price and barcode, because a crate is priced
/// below the bottles it holds and scans to a different code.
/// </para>
/// </remarks>
public sealed class Packaging
{
    private Packaging(
        string unitCode,
        int quantityInBaseUnit,
        Money sellingPrice,
        Money? costPrice,
        string? barcode)
    {
        UnitCode = unitCode;
        QuantityInBaseUnit = quantityInBaseUnit;
        SellingPrice = sellingPrice;
        CostPrice = costPrice;
        Barcode = barcode;
    }

    private Packaging()
    {
    }

    /// <summary>Unit of measure code, for example <c>CRATE</c> or <c>BOX</c>.</summary>
    public string UnitCode { get; private set; } = string.Empty;

    /// <summary>How many base units this packaging holds. Always positive.</summary>
    public int QuantityInBaseUnit { get; private set; }

    public Money SellingPrice { get; private set; } = null!;

    /// <summary>What the business pays for one of these. Absent until known.</summary>
    public Money? CostPrice { get; private set; }

    /// <summary>Scanned code for this packaging, distinct from the base unit's.</summary>
    public string? Barcode { get; private set; }

    /// <summary>
    /// Whether the packaging can be sold in quarters.
    /// </summary>
    /// <remarks>
    /// A quarter must resolve to whole base units: a crate of 24 quarters into 6
    /// bottles, but a crate of 10 would quarter into two and a half. Products whose
    /// packaging cannot be quartered simply do not offer the option.
    /// </remarks>
    public bool AllowsQuarters => QuantityInBaseUnit % 4 == 0;

    public static Packaging Create(
        string unitCode,
        int quantityInBaseUnit,
        Money sellingPrice,
        Money? costPrice = null,
        string? barcode = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(unitCode);
        ArgumentNullException.ThrowIfNull(sellingPrice);

        if (quantityInBaseUnit <= 0)
        {
            throw new DomainException(Error.Validation(ErrorCodes.NegativeAmount));
        }

        if (sellingPrice.IsNegative || costPrice?.IsNegative == true)
        {
            throw new DomainException(Error.Validation(ErrorCodes.NegativeAmount));
        }

        return new Packaging(
            unitCode.Trim().ToUpperInvariant(),
            quantityInBaseUnit,
            sellingPrice,
            costPrice,
            string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim());
    }

    /// <summary>Converts a quantity expressed in this packaging into base units.</summary>
    /// <remarks>
    /// Fractional quantities are how part-crates are sold (0.25, 0.5, 0.75). The
    /// result must land on a whole base unit; anything else is rejected rather than
    /// rounded, because half a bottle cannot leave the depot.
    /// </remarks>
    public int ToBaseUnits(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException(Error.Validation(ErrorCodes.NegativeAmount));
        }

        var baseUnits = quantity * QuantityInBaseUnit;
        if (baseUnits != decimal.Truncate(baseUnits))
        {
            throw new DomainException(Error.Validation(CatalogErrorCodes.FractionalBaseUnit));
        }

        return (int)baseUnits;
    }
}
