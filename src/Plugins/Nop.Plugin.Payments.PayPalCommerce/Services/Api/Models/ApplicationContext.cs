using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the application context
/// </summary>
public class ApplicationContext
{
    #region Properties

    /// <summary>
    /// Gets or sets the label that overrides the business name in the PayPal account on the PayPal site.
    /// </summary>
    [Obsolete("The fields in `application_context` are now available in the `experience_context` object under the `payment_source` which supports them (eg. `payment_source.paypal.experience_context.brand_name`). Please specify this field in the `experience_context` object instead of the `application_context` object.")]
    [JsonProperty(PropertyName = "brand_name")]
    public string BrandName { get; set; }

    /// <summary>
    /// Gets or sets the type of landing page to show on the PayPal site for customer checkout.
    /// </summary>
    [Obsolete("The fields in `application_context` are now available in the `experience_context` object under the `payment_source` which supports them (eg. `payment_source.paypal.experience_context.landing_page`). Please specify this field in the `experience_context` object instead of the `application_context` object.")]
    [JsonProperty(PropertyName = "landing_page")]
    public string LandingPage { get; set; }

    /// <summary>
    /// Gets or sets the shipping preference.
    /// </summary>
    [Obsolete("The fields in `application_context` are now available in the `experience_context` object under the `payment_source` which supports them (eg. `payment_source.paypal.experience_context.shipping_preference`). Please specify this field in the `experience_context` object instead of the `application_context` object.")]
    [JsonProperty(PropertyName = "shipping_preference")]
    public string ShippingPreference { get; set; }

    /// <summary>
    /// Gets or sets the user action. Configures a Continue or Pay Now checkout flow.
    /// </summary>
    [Obsolete("The fields in `application_context` are now available in the `experience_context` object under the `payment_source` which supports them (eg. `payment_source.paypal.experience_context.user_action`). Please specify this field in the `experience_context` object instead of the `application_context` object.")]
    [JsonProperty(PropertyName = "user_action")]
    public string UserAction { get; set; }

    /// <summary>
    /// Gets or sets the URL where the customer is redirected after the customer approves the payment.
    /// </summary>
    [Obsolete("The fields in `application_context` are now available in the `experience_context` object under the `payment_source` which supports them (eg. `payment_source.paypal.experience_context.return_url`). Please specify this field in the `experience_context` object instead of the `application_context` object.")]
    [JsonProperty(PropertyName = "return_url")]
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL where the customer is redirected after the customer cancels the payment.
    /// </summary>
    [Obsolete("The fields in `application_context` are now available in the `experience_context` object under the `payment_source` which supports them (eg. `payment_source.paypal.experience_context.cancel_url`). Please specify this field in the `experience_context` object instead of the `application_context` object.")]
    [JsonProperty(PropertyName = "cancel_url")]
    public string CancelUrl { get; set; }

    /// <summary>
    /// Gets or sets the BCP 47-formatted locale of pages that the PayPal payment experience shows. PayPal supports a five-character code. For example, da-DK, he-IL, id-ID, ja-JP, no-NO, pt-BR, ru-RU, sv-SE, th-TH, zh-CN, zh-HK, or zh-TW.
    /// </summary>
    [Obsolete("The fields in `application_context` are now available in the `experience_context` object under the `payment_source` which supports them (eg. `payment_source.paypal.experience_context.locale`). Please specify this field in the `experience_context` object instead of the `application_context` object.")]
    [JsonProperty(PropertyName = "locale")]
    public string Locale { get; set; }

    #endregion
}