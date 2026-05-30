using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the one time charge details for recurring payments
/// </summary>
public class OneTimeCharge
{
    #region Properties

    /// <summary>
    /// Gets or sets the setup fee for the recurring plan.
    /// </summary>
    [JsonProperty(PropertyName = "setup_fee")]
    public Money SetupFee { get; set; }

    /// <summary>
    /// Gets or sets the shipping amount due at the time of checkout.
    /// </summary>
    [JsonProperty(PropertyName = "shipping_amount")]
    public Money ShippingAmount { get; set; }

    /// <summary>
    /// Gets or sets the taxes due at the time of checkout.
    /// </summary>
    [JsonProperty(PropertyName = "taxes")]
    public Money Taxes { get; set; }

    /// <summary>
    /// Gets or sets the product price for any one-time product purchased at the time of checkout.
    /// </summary>
    [JsonProperty(PropertyName = "product_price")]
    public Money ProductPrice { get; set; }

    /// <summary>
    /// Gets or sets the total amount at the time of checkout. This is calculated by using the formula x + y + z where x is subtotal, y is shipping-amount and z is taxes.
    /// </summary>
    [JsonProperty(PropertyName = "total_amount")]
    public Money TotalAmount { get; set; }

    #endregion
}