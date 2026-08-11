namespace ConfiOS.Modules.Catalog.Domain;

/// <summary>
/// Error codes owned by the Catalog module. Each needs a matching resource entry in
/// every supported language.
/// </summary>
public static class CatalogErrorCodes
{
    public const string ProductSkuTaken = "PRODUCT_SKU_TAKEN";
    public const string ProductNotFound = "PRODUCT_NOT_FOUND";
    public const string PackagingUnitTaken = "PACKAGING_UNIT_TAKEN";
    public const string PackagingIsBaseUnit = "PACKAGING_IS_BASE_UNIT";
    public const string FractionalBaseUnit = "FRACTIONAL_BASE_UNIT";
}
