using Newtonsoft.Json;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Newtonsoft.Json.Converters;

namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample order
/// </summary>
public partial class SampleOrder
{
    /// <summary>
    /// Gets or sets a value indicating whether a customer chose "pick up in store" shipping option
    /// </summary>
    public bool PickupInStore { get; set; }

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
    /// Gets or sets the VAT number (the European Union Value Added Tax)
    /// </summary>
    public string VatNumber { get; set; } = string.Empty;

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
    public decimal OrderSubTotalDiscountInclTax { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the order subtotal discount (exclude tax)
    /// </summary>
    public decimal OrderSubTotalDiscountExclTax { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the order shipping (include tax)
    /// </summary>
    public decimal OrderShippingInclTax { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the order shipping (exclude tax)
    /// </summary>
    public decimal OrderShippingExclTax { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the payment method additional fee (incl tax)
    /// </summary>
    public decimal PaymentMethodAdditionalFeeInclTax { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the payment method additional fee (exclude tax)
    /// </summary>
    public decimal PaymentMethodAdditionalFeeExclTax { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the tax rates
    /// </summary>
    public string TaxRates { get; set; }

    /// <summary>
    /// Gets or sets the order tax
    /// </summary>
    public decimal OrderTax { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the order discount (applied to order total)
    /// </summary>
    public decimal OrderDiscount { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the order total
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// Gets or sets the refunded amount
    /// </summary>
    public decimal RefundedAmount { get; set; } = decimal.Zero;

    /// <summary>
    /// Gets or sets the checkout attribute description
    /// </summary>
    public string CheckoutAttributeDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the checkout attributes in XML format
    /// </summary>
    public string CheckoutAttributesXml { get; set; } = string.Empty;

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
    public string CardType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card name
    /// </summary>
    public string CardName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card number
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the masked credit card number
    /// </summary>
    public string MaskedCreditCardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card CVV2
    /// </summary>
    public string CardCvv2 { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card expiration month
    /// </summary>
    public string CardExpirationMonth { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the card expiration year
    /// </summary>
    public string CardExpirationYear { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authorization transaction identifier
    /// </summary>
    public string AuthorizationTransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authorization transaction code
    /// </summary>
    public string AuthorizationTransactionCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the authorization transaction result
    /// </summary>
    public string AuthorizationTransactionResult { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the capture transaction identifier
    /// </summary>
    public string CaptureTransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the capture transaction result
    /// </summary>
    public string CaptureTransactionResult { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subscription transaction identifier
    /// </summary>
    public string SubscriptionTransactionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the paid date and time
    /// </summary>
    public DateTime? PaidDateUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the shipping method
    /// </summary>
    public string ShippingMethod { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the shipping rate computation method identifier or the pickup point provider identifier (if PickupInStore is true)
    /// </summary>
    public string ShippingRateComputationMethodSystemName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized CustomValues (values from ProcessPaymentRequest)
    /// </summary>
    public string CustomValuesXml { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the custom order number without prefix
    /// </summary>
    public string CustomOrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets order items
    /// </summary>
    public List<SampleOrderItem> OrderItems { get; set; } = new();

    /// <summary>
    /// Gets or sets order notes
    /// </summary>
    public List<string> OrderNotes { get; set; } = new();

    /// <summary>
    /// Gets or sets the order status
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public OrderStatus OrderStatus { get; set; }

    /// <summary>
    /// Gets or sets the payment status
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public PaymentStatus PaymentStatus { get; set; }

    /// <summary>
    /// Gets or sets the shipping status
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public ShippingStatus ShippingStatus { get; set; }

    /// <summary>
    /// Gets or sets the customer email
    /// </summary>
    public string CustomerEmail { get; set; }

    /// <summary>
    /// Gets or sets sample shipments 
    /// </summary>
    public List<SampleShipment> Shipments { get; set; } = new();

    #region Nested class

    /// <summary>
    /// Represents a sample order item
    /// </summary>
    public partial class SampleOrderItem
    {
        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (include tax)
        /// </summary>
        public decimal UnitPriceInclTax { get; set; } = decimal.Zero;

        /// <summary>
        /// Gets or sets the unit price in primary store currency (exclude tax)
        /// </summary>
        public decimal UnitPriceExclTax { get; set; } = decimal.Zero;

        /// <summary>
        /// Gets or sets the price in primary store currency (include tax)
        /// </summary>
        public decimal PriceInclTax { get; set; } = decimal.Zero;

        /// <summary>
        /// Gets or sets the price in primary store currency (exclude tax)
        /// </summary>
        public decimal PriceExclTax { get; set; } = decimal.Zero;

        /// <summary>
        /// Gets or sets the discount amount (include tax)
        /// </summary>
        public decimal DiscountAmountInclTax { get; set; } = decimal.Zero;

        /// <summary>
        /// Gets or sets the discount amount (exclude tax)
        /// </summary>
        public decimal DiscountAmountExclTax { get; set; } = decimal.Zero;

        /// <summary>
        /// Gets or sets the original cost of this order item (when an order was placed), qty 1
        /// </summary>
        public decimal OriginalProductCost { get; set; } = decimal.Zero;

        /// <summary>
        /// Gets or sets the attribute description
        /// </summary>
        public string AttributeDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product attributes in XML format
        /// </summary>
        public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the download count
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether download is activated
        /// </summary>
        public bool IsDownloadActivated { get; set; }

        /// <summary>
        /// Gets or sets a license download identifier (in case this is a downloadable product)
        /// </summary>
        public int? LicenseDownloadId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total weight of one item
        /// It's nullable for compatibility with the previous version of nopCommerce where was no such property
        /// </summary>
        public decimal? ItemWeight { get; set; }

        /// <summary>
        /// Gets or sets the rental product start date (null if it's not a rental product)
        /// </summary>
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the rental product end date (null if it's not a rental product)
        /// </summary>
        public DateTime? RentalEndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        public string ProductName { get; set; }
    }

    /// <summary>
    /// Represents a sample shipment
    /// </summary>
    public partial class SampleShipment
    {
        /// <summary>
        /// Gets or sets the tracking number of this shipment
        /// </summary>
        public string TrackingNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total weight of this shipment
        /// It's nullable for compatibility with the previous version of nopCommerce where was no such property
        /// </summary>
        public decimal? TotalWeight { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets sample shipment items
        /// </summary>
        public List<SampleShipmentItem> ShipmentItems { get; set; } = new();
    }

    /// <summary>
    /// Represents a sample shipment item
    /// </summary>
    public partial class SampleShipmentItem
    {
        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }
    }

    #endregion
}
