using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the experience context
/// </summary>
public class ExperienceContext
{
    #region Properties

    /// <summary>
    /// Gets or sets the label that overrides the business name in the PayPal account on the PayPal site. The pattern is defined by an external party and supports Unicode.
    /// </summary>
    [JsonProperty(PropertyName = "brand_name")]
    public string BrandName { get; set; }

    /// <summary>
    /// Gets or sets the location from which the shipping address is derived.
    /// </summary>
    [JsonProperty(PropertyName = "shipping_preference")]
    public string ShippingPreference { get; set; }

    /// <summary>
    /// Gets or sets the type of landing page to show on the PayPal site for customer checkout.
    /// </summary>
    [JsonProperty(PropertyName = "landing_page")]
    public string LandingPage { get; set; }

    /// <summary>
    /// Gets or sets the user action. Configures a Continue or Pay Now checkout flow.
    /// </summary>
    [JsonProperty(PropertyName = "user_action")]
    public string UserAction { get; set; }

    /// <summary>
    /// Gets or sets the merchant-preferred payment methods.
    /// </summary>
    [JsonProperty(PropertyName = "payment_method_preference")]
    public string PaymentMethodPreference { get; set; }

    /// <summary>
    /// Gets or sets the BCP 47-formatted locale of pages that the PayPal payment experience shows. PayPal supports a five-character code. For example, da-DK, he-IL, id-ID, ja-JP, no-NO, pt-BR, ru-RU, sv-SE, th-TH, zh-CN, zh-HK, or zh-TW.
    /// </summary>
    [JsonProperty(PropertyName = "locale")]
    public string Locale { get; set; }

    /// <summary>
    /// Gets or sets the URL where the customer will be redirected upon approving a payment.
    /// </summary>
    [JsonProperty(PropertyName = "return_url")]
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL where the customer will be redirected upon cancelling the payment approval.
    /// </summary>
    [JsonProperty(PropertyName = "cancel_url")]
    public string CancelUrl { get; set; }

    #endregion
}