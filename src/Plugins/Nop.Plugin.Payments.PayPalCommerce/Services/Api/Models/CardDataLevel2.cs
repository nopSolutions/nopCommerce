using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the level 2 card data
/// </summary>
public class CardDataLevel2
{
    #region Properties

    /// <summary>
    /// Gets or sets the invoice id. Use this field to pass a purchase identification value of up to 127 ASCII characters. The length of this field will be adjusted to meet network specifications (25chars for Visa and Mastercard, 17chars for Amex), and the original invoice ID will still be displayed in your existing reports.
    /// </summary>
    [JsonProperty(PropertyName = "invoice_id")]
    public string InvoiceId { get; set; }

    /// <summary>
    /// Gets or sets the tax total. Use this field to break down the amount of tax included in the total purchase amount. The value provided here will not add to the total purchase amount. The value can't be negative, and in most cases, it must be greater than zero in order to qualify for lower interchange rates.
    /// </summary>
    [JsonProperty(PropertyName = "tax_total")]
    public Money TaxTotal { get; set; }

    #endregion
}