namespace Nop.Services.Orders;

/// <summary>
/// Represent the display location of custom value enum
/// </summary>
public enum CustomValueDisplayLocation
{
    /// <summary>
    /// Billing Address related custom values
    /// </summary>
    BillingAddress,

    /// <summary>
    /// Shipping Address related custom values
    /// </summary>
    ShippingAddress,

    /// <summary>
    /// For values associated with Payment i.e. Receipt number or Invoice Number
    /// </summary>
    Payment,

    /// <summary>
    /// For values associated with Shipping i.e. Pickup Point
    /// </summary>
    Shipping
}