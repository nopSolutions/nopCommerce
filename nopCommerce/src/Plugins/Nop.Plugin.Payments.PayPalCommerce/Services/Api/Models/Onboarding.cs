using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the merchant onboarding details
/// </summary>
public class Onboarding
{
    #region Properties

    /// <summary>
    /// Gets or sets the partner PayPal ID or Merchant ID.
    /// </summary>
    [JsonProperty(PropertyName = "partnerId")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the main PayPal product to which the partner wants to onboard the customer.
    /// </summary>
    [JsonProperty(PropertyName = "product")]
    public string Product { get; set; }

    /// <summary>
    /// Gets or sets the array of PayPal products to which the partner wants to onboard the customer.
    /// </summary>
    [JsonProperty(PropertyName = "secondaryProducts")]
    public string SecondaryProducts { get; set; }

    /// <summary>
    /// Gets or sets the array of capabilities.
    /// </summary>
    [JsonProperty(PropertyName = "capabilities")]
    public string Capabilities { get; set; }

    /// <summary>
    /// Gets or sets the array of features that partner can access, or use, in PayPal on behalf of the seller. The seller grants permission for these features to the partner.
    /// </summary>
    [JsonProperty(PropertyName = "features")]
    public string Features { get; set; }

    /// <summary>
    /// Gets or sets the integration type.
    /// </summary>
    [JsonProperty(PropertyName = "integrationType")]
    public string IntegrationType { get; set; }

    /// <summary>
    /// Gets or sets the partner Client ID.
    /// </summary>
    [JsonProperty(PropertyName = "partnerClientId")]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the URL to which to redirect the customer upon completion of the onboarding process.
    /// </summary>
    [JsonProperty(PropertyName = "returnToPartnerUrl")]
    public string ReturnToUrl { get; set; }

    /// <summary>
    /// Gets or sets the partner logo URL to display in the customer's onboarding flow.
    /// </summary>
    [JsonProperty(PropertyName = "partnerLogoUrl")]
    public string LogoUrl { get; set; }

    /// <summary>
    /// Gets or sets the display mode.
    /// </summary>
    [JsonProperty(PropertyName = "displayMode")]
    public string DisplayMode { get; set; }

    /// <summary>
    /// Gets or sets the random number (more than 40 chars) to be generated per invocation.
    /// </summary>
    [JsonProperty(PropertyName = "sellerNonce")]
    public string SellerNonce { get; set; }

    #endregion
}