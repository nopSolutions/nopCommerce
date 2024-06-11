using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the PayPal JavaScript SDK script configuration
/// </summary>
public class Script
{
    #region Properties

    /// <summary>
    /// Gets or sets the value that identifies the PayPal account that sets up and finalizes transactions. By default, funds from any transactions are settled into this account. While you're testing in sandbox, you can use `sb` as a shortcut.
    /// </summary>
    [JsonProperty(PropertyName = "client-id")]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the buyer country that determines which funding sources are eligible for a given buyer. Defaults to the buyer's IP geolocation.
    /// </summary>
    [JsonProperty(PropertyName = "buyer-country")]
    public string BuyerCountry { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to show a `Pay Now` or `Continue` button in the Checkout flow.
    /// </summary>
    [JsonProperty(PropertyName = "commit")]
    public bool Commit { get; set; }

    /// <summary>
    /// Gets or sets the PayPal components you intend to render on your page. Each component you use must be separated by a comma (,). If you don't pass any components, the default payment `buttons` component automatically renders all eligible buttons in a single location on your page.
    /// </summary>
    [JsonProperty(PropertyName = "components")]
    public string Components { get; set; }

    /// <summary>
    /// Gets or sets the currency for the transaction. Funds are captured into your account in the specified currency.
    /// </summary>
    [JsonProperty(PropertyName = "currency")]
    public string Currency { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable debug mode. Debug mode is recommended only for testing and debugging, because it increases the size of the script and negatively impacts performance.
    /// </summary>
    [JsonProperty(PropertyName = "debug")]
    public bool Debug { get; set; }

    /// <summary>
    /// Gets or sets the disabled funding sources for the transaction. Any funding sources that you pass aren't displayed as buttons at checkout. Don't use this parameter to disable advanced credit and debit card payments.
    /// </summary>
    [JsonProperty(PropertyName = "disable-funding")]
    public string DisableFunding { get; set; }

    /// <summary>
    /// Gets or sets the enabled funding sources for the transaction. Enable funding can be used to ensure a funding source is rendered, if eligible.
    /// </summary>
    [JsonProperty(PropertyName = "enable-funding")]
    public string EnableFunding { get; set; }

    /// <summary>
    /// Gets or sets the integration date of the script, passed as a `YYYY-MM-DD` value.
    /// </summary>
    [JsonProperty(PropertyName = "integration-date")]
    public string IntegrationDate { get; set; }

    /// <summary>
    /// Gets or sets the intent for the transaction. This determines whether the funds are captured immediately while the buyer is present on the page.
    /// </summary>
    [JsonProperty(PropertyName = "intent")]
    public string Intent { get; set; }

    /// <summary>
    /// Gets or sets the locale renders components. It is recommended to pass this parameter only if you need the PayPal buttons to render in the same language as the rest of your site.
    /// </summary>
    [JsonProperty(PropertyName = "locale")]
    public string Locale { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the payment information in the transaction will be saved. Save your customers' payment information for billing agreements, subscriptions, or recurring payments.
    /// </summary>
    [JsonProperty(PropertyName = "vault")]
    public bool Vault { get; set; }

    #endregion
}