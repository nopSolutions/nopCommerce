namespace Nop.Services.Common;

/// <summary>
/// Represent the display location of custom value enum
/// </summary>
public enum CustomValueDisplayLocation
{
    /// <summary>
    /// Billing Address related customer values
    /// </summary>
    BillingAddress,
    /// <summary>
    /// Shipping Address related customer values
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