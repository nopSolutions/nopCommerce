using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the purchase item
/// </summary>
public class Item
{
    #region Properties

    /// <summary>
    /// Gets or sets the item name or title.
    /// </summary>RGKD
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the item quantity. Must be a whole number.
    /// </summary>
    [JsonProperty(PropertyName = "quantity")]
    public string Quantity { get; set; }

    /// <summary>
    /// Gets or sets the detailed item description.
    /// </summary>
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the stock keeping unit (SKU) for the item.
    /// </summary>
    [JsonProperty(PropertyName = "sku")]
    public string Sku { get; set; }

    /// <summary>
    /// Gets or sets the URL to the item being purchased. Visible to buyer and used in buyer experiences.
    /// </summary>
    [JsonProperty(PropertyName = "url")]
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the tem category type.
    /// </summary>
    [JsonProperty(PropertyName = "category")]
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the URL of the item's image. File type and size restrictions apply. An image that violates these restrictions will not be honored.
    /// </summary>
    [JsonProperty(PropertyName = "image_url")]
    public string ImageUrl { get; set; }

    /// <summary>
    /// Gets or sets the Universal Product Code of the item.
    /// </summary>
    [JsonProperty(PropertyName = "upc")]
    public UniversalProductCode Upc { get; set; }

    /// <summary>
    /// Gets or sets the code used to classify items purchased and track the total amount spent across various categories of products and services. Different corporate purchasing organizations may use different standards, but the United Nations Standard Products and Services Code (UNSPSC) is frequently used.
    /// </summary>
    [JsonProperty(PropertyName = "commodity_code")]
    public string CommodityCode { get; set; }

    /// <summary>
    /// Gets or sets the unit of measure is a standard used to express the magnitude of a quantity in international trade. Most commonly used (but not limited to) examples are: Acre (ACR), Ampere (AMP), Centigram (CGM), Centimetre (CMT), Cubic inch (INQ), Cubic metre (MTQ), Fluid ounce (OZA), Foot (FOT), Hour (HUR), Item (ITM), Kilogram (KGM), Kilometre (KMT), Kilowatt (KWT), Liquid gallon (GLL), Liter (LTR), Pounds (LBS), Square foot (FTK).
    /// </summary>
    [JsonProperty(PropertyName = "unit_of_measure")]
    public string UnitOfMeasure { get; set; }

    /// <summary>
    /// Gets or sets the item price or rate per unit. Must equal `unit_amount * quantity` for all items. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "unit_amount")]
    public Money UnitAmount { get; set; }

    /// <summary>
    /// Gets or sets the item tax for each unit. Must equal `tax * quantity` for all items.
    /// </summary>
    [JsonProperty(PropertyName = "tax")]
    public Money Tax { get; set; }

    /// <summary>
    /// Gets or sets the discount amount. Use this field to break down the discount amount included in the total purchase amount. The value provided here will not add to the total purchase amount. The value cannot be negative.
    /// </summary>
    [JsonProperty(PropertyName = "discount_amount")]
    public Money DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the subtotal for all items. Must equal the sum of `(items[].unit_amount * items[].quantity)` for all items. Can not be a negative number.
    /// </summary>
    [JsonProperty(PropertyName = "total_amount")]
    public Money TotalAmount { get; set; }

    #endregion
}