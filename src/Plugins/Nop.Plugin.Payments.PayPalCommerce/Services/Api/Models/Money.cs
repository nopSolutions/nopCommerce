using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the currency and amount for a financial transaction, such as a balance or payment due
/// </summary>
public class Money
{
    #region Properties

    /// <summary>
    /// Gets or sets the [three-character ISO-4217 currency code](/docs/integration/direct/rest/currency-codes/) that identifies the currency.
    /// </summary>
    [JsonProperty(PropertyName = "currency_code")]
    public string CurrencyCode { get; set; }

    /// <summary>
    /// Gets or sets the value, which might be: An integer for currencies like `JPY` that are not typically fractional. A decimal fraction for currencies like `TND` that are subdivided into thousandths. For the required number of decimal places for a currency code, see [Currency Codes](/docs/integration/direct/rest/currency-codes/).
    /// </summary>
    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }

    #endregion
}