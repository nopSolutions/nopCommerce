using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

/// <summary>
/// Represents the purchase unit details
/// </summary>
public class PurchaseUnit
{
    #region Properties

    /// <summary>
    /// Gets or sets the API caller-provided external ID for the purchase unit. Required for multiple purchase units when you must update the order through `PATCH`. If you omit this value and the order contains only one purchase unit, PayPal sets this value to `default`.
    /// </summary>
    [JsonProperty(PropertyName = "reference_id")]
    public string ReferenceId { get; set; }

    /// <summary>
    /// Gets or sets the purchase description.
    /// </summary>
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the API caller-provided external ID. Used to reconcile API caller-initiated transactions with PayPal transactions. Appears in transaction and settlement reports.
    /// </summary>
    [JsonProperty(PropertyName = "custom_id")]
    public string CustomId { get; set; }

    /// <summary>
    /// Gets or sets the API caller-provided external invoice ID for this order.
    /// </summary>
    [JsonProperty(PropertyName = "invoice_id")]
    public string InvoiceId { get; set; }

    /// <summary>
    /// Gets or sets the PayPal-generated ID for the purchase unit. This ID appears in both the payer's transaction history and the emails that the payer receives. In addition, this ID is available in transaction and settlement reports that merchants and API callers can use to reconcile transactions.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the payment descriptor on account transactions on the customer's credit card statement. The maximum length of the soft descriptor is 22 characters.
    /// </summary>
    [JsonProperty(PropertyName = "soft_descriptor")]
    public string SoftDescriptor { get; set; }

    /// <summary>
    /// Gets or sets the array of items that the customer purchases from the merchant.
    /// </summary>
    [JsonProperty(PropertyName = "items")]
    public List<Item> Items { get; set; }

    /// <summary>
    /// Gets or sets the total order amount with an optional breakdown that provides details, such as the total item amount, total tax amount, shipping, handling, insurance, and discounts, if any. If you specify `amount.breakdown`, the amount equals `item_total` plus `tax_total` plus `shipping` plus `handling` plus `insurance` minus `shipping_discount` minus `discount`. The amount must be a positive number.
    /// </summary>
    [JsonProperty(PropertyName = "amount")]
    public OrderMoney Amount { get; set; }

    /// <summary>
    /// Gets or sets the merchant who receives payment for this transaction.
    /// </summary>
    [JsonProperty(PropertyName = "payee")]
    public Payee Payee { get; set; }

    /// <summary>
    /// Gets or sets the additional payment instructions to be consider during payment processing. This processing instruction is applicable for Capturing an order or Authorizing an Order.
    /// </summary>
    [JsonProperty(PropertyName = "payment_instruction")]
    public PaymentInstruction PaymentInstruction { get; set; }

    /// <summary>
    /// Gets or sets the shipping address and method.
    /// </summary>
    [JsonProperty(PropertyName = "shipping")]
    public Shipping Shipping { get; set; }

    /// <summary>
    /// Gets or sets the supplementary data about this payment. Merchants and partners can add Level 2 and 3 data to payments to reduce risk and payment processing costs.
    /// </summary>
    [JsonProperty(PropertyName = "supplementary_data")]
    public SupplementaryData SupplementaryData { get; set; }

    /// <summary>
    /// Gets or sets the comprehensive history of payments for the purchase unit.
    /// </summary>
    [JsonProperty(PropertyName = "payments")]
    public Payments Payments { get; set; }

    #endregion
}