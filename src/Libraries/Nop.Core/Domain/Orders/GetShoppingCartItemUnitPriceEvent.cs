using Nop.Core.Domain.Discounts;
using Nop.Core.Events;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Shopping cart item get unit price event
/// </summary>
public partial class GetShoppingCartItemUnitPriceEvent : IStopProcessingEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shoppingCartItem">Shopping cart item</param>
    /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
    public GetShoppingCartItemUnitPriceEvent(ShoppingCartItem shoppingCartItem, bool includeDiscounts)
    {
        ShoppingCartItem = shoppingCartItem;
        IncludeDiscounts = includeDiscounts;
    }

    /// <summary>
    /// Gets a value indicating whether include discounts or not for price computation
    /// </summary>
    public bool IncludeDiscounts { get; }

    /// <summary>
    /// Gets the shopping cart item
    /// </summary>
    public ShoppingCartItem ShoppingCartItem { get; }

    /// <summary>
    /// Gets or sets a value whether processing of event publishing should be stopped
    /// </summary>
    public bool StopProcessing { get; set; }

    /// <summary>
    /// Gets or sets the unit price in primary store currency
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the applied discounts
    /// </summary>
    public List<Discount> AppliedDiscounts { get; set; } = new();
}