namespace Nop.Core.Domain.PriceLists;

/// <summary>
/// Represents a price list customer mapping
/// </summary>
public partial class PriceListCustomer : BaseEntity
{
    /// <summary>
    ///  Gets or sets the price list identifier
    /// </summary>
    public int PriceListId { get; set; }

    /// <summary>
    ///  Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }
}
