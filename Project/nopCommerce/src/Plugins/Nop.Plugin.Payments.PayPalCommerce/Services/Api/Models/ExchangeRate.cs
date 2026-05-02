using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the exchange rate that determines the amount to convert from one currency to another currency
/// </summary>
public class ExchangeRate
{
    #region Properties

    /// <summary>
    /// Gets or sets the source currency from which to convert an amount.
    /// </summary>
    [JsonProperty(PropertyName = "source_currency")]
    public string SourceCurrency { get; set; }

    /// <summary>
    /// Gets or sets the target currency to which to convert an amount.
    /// </summary>
    [JsonProperty(PropertyName = "target_currency")]
    public string TargetCurrency { get; set; }

    /// <summary>
    /// Gets or sets the target currency amount. Equivalent to one unit of the source currency. Formatted as integer or decimal value with one to 15 digits to the right of the decimal point.
    /// </summary>
    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

    #endregion
}