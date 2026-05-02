using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the level 3 card data
/// </summary>
public class CardDataLevel3
{
    #region Properties

    /// <summary>
    /// Gets or sets the postal code of the shipping location.
    /// </summary>
    [JsonProperty(PropertyName = "ships_from_postal_code")]
    public string ShipsFromPostalCode { get; set; }

    /// <summary>
    /// Gets or sets the list of the items that were purchased with this payment. If your merchant account has been configured for Level 3 processing this field will be passed to the processor on your behalf.
    /// </summary>
    [JsonProperty(PropertyName = "line_items")]
    public List<Item> LineItems { get; set; }

    /// <summary>
    /// Gets or sets the shipping amount. Use this field to break down the shipping cost included in the total purchase amount. The value provided here will not add to the total purchase amount. The value cannot be negative.
    /// </summary>
    [JsonProperty(PropertyName = "shipping_amount")]
    public Money ShippingAmount { get; set; }

    /// <summary>
    /// Gets or sets the duty amount. Use this field to break down the duty amount included in the total purchase amount. The value provided here will not add to the total purchase amount. The value cannot be negative.
    /// </summary>
    [JsonProperty(PropertyName = "duty_amount")]
    public Money DutyAmount { get; set; }

    /// <summary>
    /// Gets or sets the discount amount. Use this field to break down the discount amount included in the total purchase amount. The value provided here will not add to the total purchase amount. The value cannot be negative.
    /// </summary>
    [JsonProperty(PropertyName = "discount_amount")]
    public Money DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the address of the person to whom to ship the items.
    /// </summary>
    [JsonProperty(PropertyName = "shipping_address")]
    public Address ShippingAddress { get; set; }

    #endregion
}