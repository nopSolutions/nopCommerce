using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts;

/// <summary>
/// Represents a price with discount for particular quantity
/// </summary>
public partial class DiscountPriceByQuantity
{
    /// <summary>
    /// Gets or sets a price
    /// </summary>
    public decimal DiscountPrice { get; set; }

    /// <summary>
    /// Gets or sets a quantity for which is used a certain price with discount
    /// </summary>
    public int Quantity { get; set; }

}
