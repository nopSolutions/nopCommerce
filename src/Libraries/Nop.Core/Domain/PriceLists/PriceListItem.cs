namespace Nop.Core.Domain.PriceLists;

/// <summary>
/// Represents a price list item
/// </summary>
public partial class PriceListItem : BaseEntity
{
    /// <summary>
    ///  Gets or sets the price list identifier
    /// </summary>
    public int PriceListId { get; set; }

    /// <summary>
    ///  Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the manual price (if specified, it overrides the calculated price)
    /// </summary>
    public decimal? ManualPrice { get; set; }
}
