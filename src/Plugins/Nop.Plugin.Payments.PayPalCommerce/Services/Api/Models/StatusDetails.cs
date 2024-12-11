using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the details of the transaction status
/// </summary>
public class StatusDetails
{
    #region Properties

    /// <summary>
    /// Gets or sets the reason why the transaction is in the particular status.
    /// </summary>
    [JsonProperty(PropertyName = "reason")]
    public string Reason { get; set; }

    #endregion
}