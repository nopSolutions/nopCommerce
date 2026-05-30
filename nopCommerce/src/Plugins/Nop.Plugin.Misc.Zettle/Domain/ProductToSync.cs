namespace Nop.Plugin.Misc.Zettle.Domain;

/// <summary>
/// Represents the product details ready for synchronization
/// </summary>
public class ProductToSync
{
    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1
    /// </summary>
    public string Uuid { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier as UUID version 1 of product variant
    /// </summary>
    public string VariantUuid { get; set; }

    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the product SKU
    /// </summary>
    public string Sku { get; set; }

    /// <summary>
    /// Gets or sets the product description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the original product cost
    /// </summary>
    public decimal ProductCost { get; set; }

    /// <summary>
    /// Gets or sets the category name
    /// </summary>
    public string CategoryName { get; set; }

    /// <summary>
    /// Gets or sets the image URL
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to sync images for this product
    /// </summary>
    public bool ImageSyncEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to sync price for this product
    /// </summary>
    public bool PriceSyncEnabled { get; set; }
}