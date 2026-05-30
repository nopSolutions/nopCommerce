using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the breakdown of the amount
/// </summary>
public class OrderAmountBreakdown
{
    #region Properties

    /// <summary>
    /// Gets or sets the subtotal for all items. Required if the request includes `purchase_units[].items[].unit_amount`. Must equal the sum of `(items[].unit_amount * items[].quantity)` for all items. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "item_total")]
    public Money ItemTotal { get; set; }

    /// <summary>
    /// Gets or sets the shipping fee for all items within a given `purchase_unit`. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "shipping")]
    public Money Shipping { get; set; }

    /// <summary>
    /// Gets or sets the handling fee for all items within a given `purchase_unit`. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "handling")]
    public Money Handling { get; set; }

    /// <summary>
    /// Gets or sets the total tax for all items. Required if the request includes `purchase_units.items.tax`. Must equal the sum of `(items[].tax * items[].quantity)` for all items. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "tax_total")]
    public Money TaxTotal { get; set; }

    /// <summary>
    /// Gets or sets the insurance fee for all items within a given `purchase_unit`. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "insurance")]
    public Money Insurance { get; set; }

    /// <summary>
    /// Gets or sets the shipping discount for all items within a given `purchase_unit`. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "shipping_discount")]
    public Money ShippingDiscount { get; set; }

    /// <summary>
    /// Gets or sets the discount for all items within a given `purchase_unit`. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "discount")]
    public Money Discount { get; set; }

    #endregion
}