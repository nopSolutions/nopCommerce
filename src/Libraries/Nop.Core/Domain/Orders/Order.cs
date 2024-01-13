using Nop.Core.Domain.Common;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Represents an order
/// </summary>
public partial class Order : BaseEntity, ISoftDeletedEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the order identifier
    /// </summary>
    public Guid OrderGuid { get; set; }

    /// <summary>
    /// Gets or sets the store identifier
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the billing address identifier
    /// </summary>
    public int BillingAddressId { get; set; }

    /// <summary>
    /// Gets or sets the shipping address identifier
    /// </summary>
    public int? ShippingAddressId { get; set; }

    /// <summary>
    /// Gets or sets the pickup address identifier
    /// </summary>
    public int? PickupAddressId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a customer chose "pick up in store" shipping option
    /// </summary>
    public bool PickupInStore { get; set; }

    /// <summary>
    /// Gets or sets an order status identifier
    /// </summary>
    public int OrderStatusId { get; set; }

    /// <summary>
    /// Gets or sets the shipping status identifier
    /// </summary>
    public int ShippingStatusId { get; set; }

    /// <summary>
    /// Gets or sets the payment status identifier
    /// </summary>
    public int PaymentStatusId { get; set; }

    /// <summary>
    /// Gets or sets the payment method system name
    /// </summary>
    public string PaymentMethodSystemName { get; set; }

    /// <summary>
    /// Gets or sets the customer currency code (at the moment of order placing)
    /// </summary>
    public string CustomerCurrencyCode { get; set; }

    /// <summary>
    /// Gets or sets the currency rate
    /// </summary>
    public decimal CurrencyRate { get; set; }

    /// <summary>
    /// Gets or sets the customer tax display type identifier
    /// </summary>
    public int CustomerTaxDisplayTypeId { get; set; }

    /// <summary>
    /// Gets or sets the VAT number (the European Union Value Added Tax)
    /// </summary>
    public string VatNumber { get; set; }

    /// <summary>
    /// Gets or sets the order subtotal (include tax)
    /// </summary>
    public decimal OrderSubtotalInclTax { get; set; }

    /// <summary>
    /// Gets or sets the order subtotal (exclude tax)
    /// </summary>
    public decimal OrderSubtotalExclTax { get; set; }

    /// <summary>
    /// Gets or sets the order subtotal discount (include tax)
    /// </summary>
    public decimal OrderSubTotalDiscountInclTax { get; set; }

    /// <summary>
    /// Gets or sets the order subtotal discount (exclude tax)
    /// </summary>
    public decimal OrderSubTotalDiscountExclTax { get; set; }

    /// <summary>
    /// Gets or sets the order shipping (include tax)
    /// </summary>
    public decimal OrderShippingInclTax { get; set; }

    /// <summary>
    /// Gets or sets the order shipping (exclude tax)
    /// </summary>
    public decimal OrderShippingExclTax { get; set; }

    /// <summary>
    /// Gets or sets the payment method additional fee (incl tax)
    /// </summary>
    public decimal PaymentMethodAdditionalFeeInclTax { get; set; }

    /// <summary>
    /// Gets or sets the payment method additional fee (exclude tax)
    /// </summary>
    public decimal PaymentMethodAdditionalFeeExclTax { get; set; }

    /// <summary>
    /// Gets or sets the tax rates
    /// </summary>
    public string TaxRates { get; set; }

    /// <summary>
    /// Gets or sets the order tax
    /// </summary>
    public decimal OrderTax { get; set; }

    /// <summary>
    /// Gets or sets the order discount (applied to order total)
    /// </summary>
    public decimal OrderDiscount { get; set; }

    /// <summary>
    /// Gets or sets the order total
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// Gets or sets the refunded amount
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// Gets or sets the reward points history entry identifier when reward points were earned (gained) for placing this order
    /// </summary>
    public int? RewardPointsHistoryEntryId { get; set; }

    /// <summary>
    /// Gets or sets the checkout attribute description
    /// </summary>
    public string CheckoutAttributeDescription { get; set; }

    /// <summary>
    /// Gets or sets the checkout attributes in XML format
    /// </summary>
    public string CheckoutAttributesXml { get; set; }

    /// <summary>
    /// Gets or sets the customer language identifier
    /// </summary>
    public int CustomerLanguageId { get; set; }

    /// <summary>
    /// Gets or sets the affiliate identifier
    /// </summary>
    public int AffiliateId { get; set; }

    /// <summary>
    /// Gets or sets the customer IP address
    /// </summary>
    public string CustomerIp { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether storing of credit card number is allowed
    /// </summary>
    public bool AllowStoringCreditCardNumber { get; set; }

    /// <summary>
    /// Gets or sets the card type
    /// </summary>
    public string CardType { get; set; }

    /// <summary>
    /// Gets or sets the card name
    /// </summary>
    public string CardName { get; set; }

    /// <summary>
    /// Gets or sets the card number
    /// </summary>
    public string CardNumber { get; set; }

    /// <summary>
    /// Gets or sets the masked credit card number
    /// </summary>
    public string MaskedCreditCardNumber { get; set; }

    /// <summary>
    /// Gets or sets the card CVV2
    /// </summary>
    public string CardCvv2 { get; set; }

    /// <summary>
    /// Gets or sets the card expiration month
    /// </summary>
    public string CardExpirationMonth { get; set; }

    /// <summary>
    /// Gets or sets the card expiration year
    /// </summary>
    public string CardExpirationYear { get; set; }

    /// <summary>
    /// Gets or sets the authorization transaction identifier
    /// </summary>
    public string AuthorizationTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the authorization transaction code
    /// </summary>
    public string AuthorizationTransactionCode { get; set; }

    /// <summary>
    /// Gets or sets the authorization transaction result
    /// </summary>
    public string AuthorizationTransactionResult { get; set; }

    /// <summary>
    /// Gets or sets the capture transaction identifier
    /// </summary>
    public string CaptureTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the capture transaction result
    /// </summary>
    public string CaptureTransactionResult { get; set; }

    /// <summary>
    /// Gets or sets the subscription transaction identifier
    /// </summary>
    public string SubscriptionTransactionId { get; set; }

    /// <summary>
    /// Gets or sets the paid date and time
    /// </summary>
    public DateTime? PaidDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the shipping method
    /// </summary>
    public string ShippingMethod { get; set; }

    /// <summary>
    /// Gets or sets the shipping rate computation method identifier or the pickup point provider identifier (if PickupInStore is true)
    /// </summary>
    public string ShippingRateComputationMethodSystemName { get; set; }

    /// <summary>
    /// Gets or sets the serialized CustomValues (values from ProcessPaymentRequest)
    /// </summary>
    public string CustomValuesXml { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been deleted
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time of order creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the custom order number without prefix
    /// </summary>
    public string CustomOrderNumber { get; set; }

    /// <summary>
    /// Gets or sets the reward points history record (spent by a customer when placing this order)
    /// </summary>
    public virtual int? RedeemedRewardPointsEntryId { get; set; }

    #endregion

    #region Custom properties

    /// <summary>
    /// Gets or sets the order status
    /// </summary>
    public OrderStatus OrderStatus
    {
        get => (OrderStatus)OrderStatusId;
        set => OrderStatusId = (int)value;
    }

    /// <summary>
    /// Gets or sets the payment status
    /// </summary>
    public PaymentStatus PaymentStatus
    {
        get => (PaymentStatus)PaymentStatusId;
        set => PaymentStatusId = (int)value;
    }

    /// <summary>
    /// Gets or sets the shipping status
    /// </summary>
    public ShippingStatus ShippingStatus
    {
        get => (ShippingStatus)ShippingStatusId;
        set => ShippingStatusId = (int)value;
    }

    /// <summary>
    /// Gets or sets the customer tax display type
    /// </summary>
    public TaxDisplayType CustomerTaxDisplayType
    {
        get => (TaxDisplayType)CustomerTaxDisplayTypeId;
        set => CustomerTaxDisplayTypeId = (int)value;
    }

    #endregion
}