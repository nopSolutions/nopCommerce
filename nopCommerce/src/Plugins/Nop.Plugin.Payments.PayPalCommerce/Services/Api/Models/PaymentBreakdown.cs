using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the detailed breakdown of the transaction
/// </summary>
public class PaymentBreakdown
{
    #region Properties

    /// <summary>
    /// Gets or sets the array of platform or partner fees, commissions, or brokerage fees that associated with the transaction.
    /// </summary>
    [JsonProperty(PropertyName = "platform_fees")]
    public List<PlatformFee> PlatformFees { get; set; }

    /// <summary>
    /// Gets or sets the amount for this transaction.
    /// </summary>
    [JsonProperty(PropertyName = "gross_amount")]
    public Money GrossAmount { get; set; }

    /// <summary>
    /// Gets or sets the applicable fee for this transaction.
    /// </summary>
    [JsonProperty(PropertyName = "paypal_fee")]
    public Money PaypalFee { get; set; }

    /// <summary>
    /// Gets or sets the applicable fee for this transaction in the receivable currency. Returned only in cases the fee is charged in the receivable currency. Example 'CNY'.
    /// </summary>
    [JsonProperty(PropertyName = "paypal_fee_in_receivable_currency")]
    public Money PaypalFeeInReceivableCurrency { get; set; }

    /// <summary>
    /// Gets or sets the net amount that the payee receives for this transaction in their PayPal account. The net amount is computed as `gross_amount` minus the `paypal_fee` minus the `platform_fees`.
    /// </summary>
    [JsonProperty(PropertyName = "net_amount")]
    public Money NetAmount { get; set; }

    /// <summary>
    /// Gets or sets the net amount that the payee's account is debited in the receivable currency. Returned only in cases when the receivable currency is different from transaction currency. Example 'CNY'.
    /// </summary>
    [JsonProperty(PropertyName = "net_amount_in_receivable_currency")]
    public Money NetAmountInReceivableCurrency { get; set; }

    /// <summary>
    /// Gets or sets the net amount that is credited to the payee's PayPal account. Returned only when the currency of the transaction is different from the currency of the PayPal account where the payee wants to credit the funds. The amount is computed as `net_amount` times `exchange_rate`.
    /// </summary>
    [JsonProperty(PropertyName = "receivable_amount")]
    public Money ReceivableAmount { get; set; }

    /// <summary>
    /// Gets or sets the exchange rate that determines the amount that is credited to the payee's PayPal account. Returned when the currency of the transaction is different from the currency of the PayPal account where the payee wants to credit the funds.
    /// </summary>
    [JsonProperty(PropertyName = "exchange_rate")]
    public ExchangeRate ExchangeRate { get; set; }

    /// <summary>
    /// Gets or sets the total amount refunded from the original capture to date. For example, if a payer makes a $100 purchase and was refunded $20 a week ago and was refunded $30 in this refund, the `gross_amount` is $30 for this refund and the `total_refunded_amount` is $50.
    /// </summary>
    [JsonProperty(PropertyName = "total_refunded_amount")]
    public Money TotalRefundedAmount { get; set; }

    /// <summary>
    /// Gets or sets the array of breakdown values for the net amount. Returned when the currency of the refund is different from the currency of the PayPal account where the payee holds their funds.
    /// </summary>
    [JsonProperty(PropertyName = "net_amount_breakdown")]
    public List<NetAmountBreakdown> NetAmountBreakdown { get; set; }

    #endregion
}