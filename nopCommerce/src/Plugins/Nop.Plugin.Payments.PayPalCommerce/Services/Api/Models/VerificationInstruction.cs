using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the instruction to verify the card
/// </summary>
public class VerificationInstruction
{
    #region Properties

    /// <summary>
    /// Gets or sets the method used for card verification.
    /// </summary>
    [JsonProperty(PropertyName = "method")]
    public string Method { get; set; }

    #endregion
}