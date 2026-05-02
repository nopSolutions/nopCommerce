namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample manufacturer
/// </summary>
public partial class SampleManufacturer
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the picture file name
    /// </summary>
    public string ImageFileName { get; set; }

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether customers can select the page size
    /// </summary>
    public bool AllowCustomersToSelectPageSize { get; set; }

    /// <summary>
    /// Gets or sets the available customer selectable page size options
    /// </summary>
    public string PageSizeOptions { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is published
    /// </summary>
    public bool Published { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the price range filtering is enabled
    /// </summary>
    public bool PriceRangeFiltering { get; set; }

    /// <summary>
    /// Gets or sets the "from" price
    /// </summary>
    public decimal PriceFrom { get; set; }

    /// <summary>
    /// Gets or sets the "to" price
    /// </summary>
    public decimal PriceTo { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the price range should be entered manually
    /// </summary>
    public bool ManuallyPriceRange { get; set; }
}
