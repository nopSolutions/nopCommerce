namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents the product URL structure type enum
/// </summary>
public enum ProductUrlStructureType
{
    /// <summary>
    /// Product only (e.g. '/product-seo-name')
    /// </summary>
    Product = 0,

    /// <summary>
    /// Category (the most nested), then product (e.g. '/category-seo-name/product-seo-name')
    /// </summary>
    CategoryProduct = 10,

    /// <summary>
    /// Manufacturer, then product (e.g. '/manufacturer-seo-name/product-seo-name')
    /// </summary>
    ManufacturerProduct = 20
}