namespace Nop.Plugin.Payments.AmazonPay.Enums;

/// <summary>
/// Represents button placement enumeration
/// </summary>
public enum ButtonPlacement
{
    /// <summary>
    /// Cart page
    /// </summary>
    Cart = 1,

    /// <summary>
    /// Product details page
    /// </summary>
    Product = 2,

    /// <summary>
    /// First page of the checkout process
    /// </summary>
    Checkout = 3,

    /// <summary>
    /// Mini (flyout) shopping cart
    /// </summary>
    MiniCart = 4,

    /// <summary>
    /// Payment method page of the checkout process
    /// </summary>
    PaymentMethod = 5
}