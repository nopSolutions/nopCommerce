using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTO.Base;
using Nop.Plugin.Api.DTO.Customers;
using Nop.Plugin.Api.DTO.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;

namespace Nop.Plugin.Api.DTO.Orders
{
    [JsonObject(Title = "order")]
    //[Validator(typeof(OrderDtoValidator))]
    public class OrderDto : BaseDto
    {
        private ICollection<OrderItemDto> _orderItems;

        [JsonProperty("store_id")]
        public int? StoreId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a customer chose "pick up in store" shipping option
        /// </summary>
        [JsonProperty("pick_up_in_store")]
        public bool? PickUpInStore { get; set; }

        /// <summary>
        ///     Gets or sets the payment method system name
        /// </summary>
        [JsonProperty("payment_method_system_name")]
        public string PaymentMethodSystemName { get; set; }

        /// <summary>
        ///     Gets or sets the customer currency code (at the moment of order placing)
        /// </summary>
        [JsonProperty("customer_currency_code")]
        public string CustomerCurrencyCode { get; set; }

        /// <summary>
        ///     Gets or sets the currency rate
        /// </summary>
        [JsonProperty("currency_rate")]
        public decimal? CurrencyRate { get; set; }

        /// <summary>
        ///     Gets or sets the customer tax display type identifier
        /// </summary>
        [JsonProperty("customer_tax_display_type_id")]
        public int? CustomerTaxDisplayTypeId { get; set; }

        /// <summary>
        ///     Gets or sets the VAT number (the European Union Value Added Tax)
        /// </summary>
        [JsonProperty("vat_number")]
        public string VatNumber { get; set; }

        /// <summary>
        ///     Gets or sets the order subtotal (incl tax)
        /// </summary>
        [JsonProperty("order_subtotal_incl_tax")]
        public decimal? OrderSubtotalInclTax { get; set; }

        /// <summary>
        ///     Gets or sets the order subtotal (excl tax)
        /// </summary>
        [JsonProperty("order_subtotal_excl_tax")]
        public decimal? OrderSubtotalExclTax { get; set; }

        /// <summary>
        ///     Gets or sets the order subtotal discount (incl tax)
        /// </summary>
        [JsonProperty("order_sub_total_discount_incl_tax")]
        public decimal? OrderSubTotalDiscountInclTax { get; set; }

        /// <summary>
        ///     Gets or sets the order subtotal discount (excl tax)
        /// </summary>
        [JsonProperty("order_sub_total_discount_excl_tax")]
        public decimal? OrderSubTotalDiscountExclTax { get; set; }

        /// <summary>
        ///     Gets or sets the order shipping (incl tax)
        /// </summary>
        [JsonProperty("order_shipping_incl_tax")]
        public decimal? OrderShippingInclTax { get; set; }

        /// <summary>
        ///     Gets or sets the order shipping (excl tax)
        /// </summary>
        [JsonProperty("order_shipping_excl_tax")]
        public decimal? OrderShippingExclTax { get; set; }

        /// <summary>
        ///     Gets or sets the payment method additional fee (incl tax)
        /// </summary>
        [JsonProperty("payment_method_additional_fee_incl_tax")]
        public decimal? PaymentMethodAdditionalFeeInclTax { get; set; }

        /// <summary>
        ///     Gets or sets the payment method additional fee (excl tax)
        /// </summary>
        [JsonProperty("payment_method_additional_fee_excl_tax")]
        public decimal? PaymentMethodAdditionalFeeExclTax { get; set; }

        /// <summary>
        ///     Gets or sets the tax rates
        /// </summary>
        [JsonProperty("tax_rates")]
        public string TaxRates { get; set; }

        /// <summary>
        ///     Gets or sets the order tax
        /// </summary>
        [JsonProperty("order_tax")]
        public decimal? OrderTax { get; set; }

        /// <summary>
        ///     Gets or sets the order discount (applied to order total)
        /// </summary>
        [JsonProperty("order_discount")]
        public decimal? OrderDiscount { get; set; }

        /// <summary>
        ///     Gets or sets the order total
        /// </summary>
        [JsonProperty("order_total")]
        public decimal? OrderTotal { get; set; }

        /// <summary>
        ///     Gets or sets the refunded amount
        /// </summary>
        [JsonProperty("refunded_amount")]
        public decimal? RefundedAmount { get; set; }

        /// <summary>
        ///     Gets or sets the value indicating whether reward points were earned for this order
        /// </summary>
        [JsonProperty("reward_points_were_added")]
        public bool? RewardPointsWereAdded { get; set; }

        /// <summary>
        ///     Gets or sets the checkout attribute description
        /// </summary>
        [JsonProperty("checkout_attribute_description")]
        public string CheckoutAttributeDescription { get; set; }

        /// <summary>
        ///     Gets or sets the customer language identifier
        /// </summary>
        [JsonProperty("customer_language_id")]
        public int? CustomerLanguageId { get; set; }

        /// <summary>
        ///     Gets or sets the affiliate identifier
        /// </summary>
        [JsonProperty("affiliate_id")]
        public int? AffiliateId { get; set; }

        /// <summary>
        ///     Gets or sets the customer IP address
        /// </summary>
        [JsonProperty("customer_ip")]
        public string CustomerIp { get; set; }

        /// <summary>
        ///     Gets or sets the authorization transaction identifier
        /// </summary>
        [JsonProperty("authorization_transaction_id")]
        public string AuthorizationTransactionId { get; set; }

        /// <summary>
        ///     Gets or sets the authorization transaction code
        /// </summary>
        [JsonProperty("authorization_transaction_code")]
        public string AuthorizationTransactionCode { get; set; }

        /// <summary>
        ///     Gets or sets the authorization transaction result
        /// </summary>
        [JsonProperty("authorization_transaction_result")]
        public string AuthorizationTransactionResult { get; set; }

        /// <summary>
        ///     Gets or sets the capture transaction identifier
        /// </summary>
        [JsonProperty("capture_transaction_id")]
        public string CaptureTransactionId { get; set; }

        /// <summary>
        ///     Gets or sets the capture transaction result
        /// </summary>
        [JsonProperty("capture_transaction_result")]
        public string CaptureTransactionResult { get; set; }

        /// <summary>
        ///     Gets or sets the subscription transaction identifier
        /// </summary>
        [JsonProperty("subscription_transaction_id")]
        public string SubscriptionTransactionId { get; set; }

        /// <summary>
        ///     Gets or sets the paid date and time
        /// </summary>
        [JsonProperty("paid_date_utc")]
        public DateTime? PaidDateUtc { get; set; }

        /// <summary>
        ///     Gets or sets the shipping method
        /// </summary>
        [JsonProperty("shipping_method")]
        public string ShippingMethod { get; set; }

        /// <summary>
        ///     Gets or sets the shipping rate computation method identifier
        /// </summary>
        [JsonProperty("shipping_rate_computation_method_system_name")]
        public string ShippingRateComputationMethodSystemName { get; set; }

        /// <summary>
        ///     Gets or sets the serialized CustomValues (values from ProcessPaymentRequest)
        /// </summary>
        [JsonProperty("custom_values_xml")]
        public string CustomValuesXml { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        [JsonProperty("deleted")]
        public bool? Deleted { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of order creation
        /// </summary>
        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }

        [JsonProperty("customer_id")]
        public int? CustomerId { get; set; }

        /// <summary>
        ///     Gets or sets the billing address
        /// </summary>
        [JsonProperty("billing_address", Required = Required.Always)]
        public AddressDto BillingAddress { get; set; }

        /// <summary>
        ///     Gets or sets the shipping address
        /// </summary>
        [JsonProperty("shipping_address")]
        public AddressDto ShippingAddress { get; set; }

        /// <summary>
        ///     Gets or sets order items
        /// </summary>
        [JsonProperty("order_items")]
        public ICollection<OrderItemDto> OrderItems
        {
            get
            {
                if (_orderItems == null)
                {
                    _orderItems = new List<OrderItemDto>();
                }

                return _orderItems;
            }
            set => _orderItems = value;
        }

        /// <summary>
        ///     Gets or sets the order status
        /// </summary>
        [JsonProperty("order_status")]
        public OrderStatus? OrderStatus { get; set; }

        /// <summary>
        ///     Gets or sets the payment status
        /// </summary>
        [JsonProperty("payment_status")]
        public PaymentStatus? PaymentStatus { get; set; }

        /// <summary>
        ///     Gets or sets the shipping status
        /// </summary>
        [JsonProperty("shipping_status")]
        public ShippingStatus? ShippingStatus { get; set; }

        /// <summary>
        ///     Gets or sets the customer tax display type
        /// </summary>
        [JsonProperty("customer_tax_display_type")]
        public string CustomerTaxDisplayType { get; set; }
    }
}
