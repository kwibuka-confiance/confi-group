namespace ConfiOS.Modules.Catalog.Domain.Authorization;

/// <summary>Permissions the Catalog module defines, named <c>catalog.resource.action</c>.</summary>
public static class CatalogPermissions
{
    public static class Products
    {
        public const string Read = "catalog.products.read";
        public const string Create = "catalog.products.create";
        public const string Update = "catalog.products.update";
    }

    /// <summary>Every permission defined by this module.</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        Products.Read, Products.Create, Products.Update,
    ];
}
