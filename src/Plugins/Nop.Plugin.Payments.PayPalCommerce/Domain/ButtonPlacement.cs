namespace Nop.Plugin.Payments.PayPalCommerce.Domain;

/// <summary>
/// Represents the button placement
/// </summary>
public enum ButtonPlacement
{
    /// <summary>
    /// Shopping cart page
    /// </summary>
    Cart,

    /// <summary>
    /// Product details page
    /// </summary>
    Product,

    /// <summary>
    /// Checkout payment method page
    /// </summary>
    PaymentMethod
}