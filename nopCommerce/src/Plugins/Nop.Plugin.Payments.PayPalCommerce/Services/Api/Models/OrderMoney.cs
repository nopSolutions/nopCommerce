using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the currency and amount for an order with the breakdown
/// </summary>
public class OrderMoney : Money
{
    #region Properties

    /// <summary>
    /// Gets or sets the breakdown of the amount. Breakdown provides details such as total item amount, total tax amount, shipping, handling, insurance, and discounts, if any.
    /// </summary>
    [JsonProperty(PropertyName = "breakdown")]
    public OrderAmountBreakdown Breakdown { get; set; }

    #endregion
}