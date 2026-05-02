using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the net amount breakdown
/// </summary>
public class NetAmountBreakdown
{
    #region Properties

    /// <summary>
    /// Gets or sets the net amount debited from the merchant's PayPal account.
    /// </summary>
    [JsonProperty(PropertyName = "payable_amount")]
    public Money PayableAmount { get; set; }

    /// <summary>
    /// Gets or sets the converted payable amount.
    /// </summary>
    [JsonProperty(PropertyName = "converted_amount")]
    public Money ConvertedAmount { get; set; }

    /// <summary>
    /// Gets or sets the exchange rate that determines the amount that is credited to the payee's PayPal account. Returned when the currency of the transaction is different from the currency of the PayPal account where the payee wants to credit the funds.
    /// </summary>
    [JsonProperty(PropertyName = "exchange_rate")]
    public ExchangeRate ExchangeRate { get; set; }

    #endregion
}