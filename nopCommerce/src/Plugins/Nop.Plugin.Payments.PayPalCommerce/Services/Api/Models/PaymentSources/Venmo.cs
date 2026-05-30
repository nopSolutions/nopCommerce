using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.PaymentSources;

/// <summary>
/// Represents the information needed to indicate that Venmo is being used to fund the payment.
/// </summary>
public class Venmo : Payer
{
    #region Properties

    /// <summary>
    /// Gets or sets the Venmo username, as chosen by the user.
    /// </summary>
    [JsonProperty(PropertyName = "user_name")]
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the additional attributes associated with the use of this wallet.
    /// </summary>
    [JsonProperty(PropertyName = "attributes")]
    public Attributes Attributes { get; set; }

    /// <summary>
    /// Gets or sets the PayPal-generated ID for the Venmo wallet payment source. Typically stored on the merchant's server.
    /// </summary>
    [JsonProperty(PropertyName = "vault_id")]
    public string VaultId { get; set; }

    /// <summary>
    /// Gets or sets the buyer experience during the approval process for payment with Venmo.
    /// </summary>
    [JsonProperty(PropertyName = "experience_context")]
    public ExperienceContext ExperienceContext { get; set; }

    #endregion
}