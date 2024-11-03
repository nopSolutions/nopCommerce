using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the platform fee
/// </summary>
public class PlatformFee
{
    #region Properties

    /// <summary>
    /// Gets or sets the fee for this transaction.
    /// </summary>
    [JsonProperty(PropertyName = "amount")]
    public Money Amount { get; set; }

    /// <summary>
    /// Gets or sets the recipient of the fee for this transaction. If you omit this value, the default is the API caller.
    /// </summary>
    [JsonProperty(PropertyName = "payee")]
    public Payee Payee { get; set; }

    #endregion
}