using Nop.Core.Domain.Discounts;

namespace Nop.Services.Discounts;

/// <summary>
/// Represents the details of result of requesting discount price
/// </summary>
public partial class DiscountPrice
{
    /// <summary>
    /// Gets or sets a price with discounts (the same for all items)
    /// </summary>
    public decimal PriceWithDiscount { get; set; }

    /// <summary>
    /// Gets or sets a discount amount (the same for all items)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    public int DiscountQuantity { get; set; }

    /// <summary>
    /// Gets or sets an applied discounts
    /// </summary>
    public List<Discount> AppliedDiscounts { get; set; } = new();

}
