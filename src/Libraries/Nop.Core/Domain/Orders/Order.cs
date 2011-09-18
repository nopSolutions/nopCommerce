using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public partial class Order : BaseEntity
    {

        private ICollection<DiscountUsageHistory> _discountUsageHistory;
        private ICollection<GiftCardUsageHistory> _giftCardUsageHistory;
        private ICollection<OrderNote> _orderNotes;
        private ICollection<OrderProductVariant> _orderProductVariant;

        #region Utilities

        protected virtual SortedDictionary<decimal, decimal> ParseTaxRates(string taxRatesStr)
        {
            var taxRatesDictionary = new SortedDictionary<decimal, decimal>();
            if (String.IsNullOrEmpty(taxRatesStr))
                return taxRatesDictionary;

            string[] lines = taxRatesStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (String.IsNullOrEmpty(line.Trim()))
                    continue;

                string[] taxes = line.Split(new char[] { ':' });
                if (taxes.Length == 2)
                {
                    try
                    {
                        decimal taxRate = decimal.Parse(taxes[0].Trim(), CultureInfo.InvariantCulture);
                        decimal taxValue = decimal.Parse(taxes[1].Trim(), CultureInfo.InvariantCulture);
                        taxRatesDictionary.Add(taxRate, taxValue);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc.ToString());
                    }
                }
            }

            //add at least one tax rate (0%)
            if (taxRatesDictionary.Count == 0)
                taxRatesDictionary.Add(decimal.Zero, decimal.Zero);

            return taxRatesDictionary;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public virtual Guid OrderGuid { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the billing address identifier
        /// </summary>
        public virtual int BillingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the shipping address identifier
        /// </summary>
        public virtual int? ShippingAddressId { get; set; }

        /// <summary>
        /// Gets or sets an order status identifer
        /// </summary>
        public virtual int OrderStatusId { get; set; }

        /// <summary>
        /// Gets or sets the shipping status identifier
        /// </summary>
        public virtual int ShippingStatusId { get; set; }

        /// <summary>
        /// Gets or sets the payment status identifier
        /// </summary>
        public virtual int PaymentStatusId { get; set; }

        /// <summary>
        /// Gets or sets the payment method system name
        /// </summary>
        public virtual string PaymentMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets the customer currency code (at the moment of order placing)
        /// </summary>
        public virtual string CustomerCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the currency rate
        /// </summary>
        public virtual decimal CurrencyRate { get; set; }

        /// <summary>
        /// Gets or sets the customer tax display type identifier
        /// </summary>
        public virtual int CustomerTaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets the VAT number (the European Union Value Added Tax)
        /// </summary>
        public virtual string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (incl tax)
        /// </summary>
        public virtual decimal OrderSubtotalInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (excl tax)
        /// </summary>
        public virtual decimal OrderSubtotalExclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount (incl tax)
        /// </summary>
        public virtual decimal OrderSubTotalDiscountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount (excl tax)
        /// </summary>
        public virtual decimal OrderSubTotalDiscountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping (incl tax)
        /// </summary>
        public virtual decimal OrderShippingInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping (excl tax)
        /// </summary>
        public virtual decimal OrderShippingExclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee (incl tax)
        /// </summary>
        public virtual decimal PaymentMethodAdditionalFeeInclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee (excl tax)
        /// </summary>
        public virtual decimal PaymentMethodAdditionalFeeExclTax { get; set; }

        /// <summary>
        /// Gets or sets the tax rates
        /// </summary>
        public virtual string TaxRates { get; set; }

        /// <summary>
        /// Gets or sets the order tax
        /// </summary>
        public virtual decimal OrderTax { get; set; }

        /// <summary>
        /// Gets or sets the order discount (applied to order total)
        /// </summary>
        public virtual decimal OrderDiscount { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        public virtual decimal OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the refunded amount
        /// </summary>
        public virtual decimal RefundedAmount { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether reward points were earned for this order
        /// </summary>
        public virtual bool RewardPointsWereAdded { get; set; }
        
        /// <summary>
        /// Gets or sets the checkout attribute description
        /// </summary>
        public virtual string CheckoutAttributeDescription { get; set; }

        /// <summary>
        /// Gets or sets the checkout attributes in XML format
        /// </summary>
        public virtual string CheckoutAttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the customer language identifier
        /// </summary>
        public virtual int CustomerLanguageId { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public virtual int? AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the customer IP address
        /// </summary>
        public virtual string CustomerIp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether storing of credit card number is allowed
        /// </summary>
        public virtual bool AllowStoringCreditCardNumber { get; set; }

        /// <summary>
        /// Gets or sets the card type
        /// </summary>
        public virtual string CardType { get; set; }

        /// <summary>
        /// Gets or sets the card name
        /// </summary>
        public virtual string CardName { get; set; }

        /// <summary>
        /// Gets or sets the card number
        /// </summary>
        public virtual string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the masked credit card number
        /// </summary>
        public virtual string MaskedCreditCardNumber { get; set; }

        /// <summary>
        /// Gets or sets the card CVV2
        /// </summary>
        public virtual string CardCvv2 { get; set; }

        /// <summary>
        /// Gets or sets the card expiration month
        /// </summary>
        public virtual string CardExpirationMonth { get; set; }

        /// <summary>
        /// Gets or sets the card expiration year
        /// </summary>
        public virtual string CardExpirationYear { get; set; }

        /// <summary>
        /// Gets or sets the authorization transaction identifier
        /// </summary>
        public virtual string AuthorizationTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the authorization transaction code
        /// </summary>
        public virtual string AuthorizationTransactionCode { get; set; }

        /// <summary>
        /// Gets or sets the authorization transaction result
        /// </summary>
        public virtual string AuthorizationTransactionResult { get; set; }

        /// <summary>
        /// Gets or sets the capture transaction identifier
        /// </summary>
        public virtual string CaptureTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the capture transaction result
        /// </summary>
        public virtual string CaptureTransactionResult { get; set; }

        /// <summary>
        /// Gets or sets the subscription transaction identifier
        /// </summary>
        public virtual string SubscriptionTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the purchase order number
        /// </summary>
        public virtual string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the paid date and time
        /// </summary>
        public virtual DateTime? PaidDateUtc { get; set; }
        
        /// <summary>
        /// Gets or sets the shipping method
        /// </summary>
        public virtual string ShippingMethod { get; set; }

        /// <summary>
        /// Gets or sets the shipping rate computation method identifier
        /// </summary>
        public virtual string ShippingRateComputationMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets the shipped date and time
        /// </summary>
        public virtual DateTime? ShippedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the delivery date and time
        /// </summary>
        public virtual DateTime? DeliveryDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the order weight
        /// </summary>
        public virtual decimal OrderWeight { get; set; }

        /// <summary>
        /// Gets or sets the tracking number of current order
        /// </summary>
        public virtual string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        #endregion

        #region Navigation properties

        /// <summary>
        /// Gets or sets the affiliate
        /// </summary>
        public virtual Affiliate Affiliate { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the billing address
        /// </summary>
        public virtual Address BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the shipping address
        /// </summary>
        public virtual Address ShippingAddress { get; set; }
        
        /// <summary>
        /// Gets or sets the reward points history record
        /// </summary>
        public virtual RewardPointsHistory RedeemedRewardPointsEntry { get; set; }

        /// <summary>
        /// Gets or sets discount usage history
        /// </summary>
        public virtual ICollection<DiscountUsageHistory> DiscountUsageHistory
        {
            get { return _discountUsageHistory ?? (_discountUsageHistory = new List<DiscountUsageHistory>()); }
            protected set { _discountUsageHistory = value; }
        }

        /// <summary>
        /// Gets or sets gift card usage history (gift card that were used with this order)
        /// </summary>
        public virtual ICollection<GiftCardUsageHistory> GiftCardUsageHistory
        {
            get { return _giftCardUsageHistory ?? (_giftCardUsageHistory = new List<GiftCardUsageHistory>()); }
            protected set { _giftCardUsageHistory = value; }
        }

        /// <summary>
        /// Gets or sets order notes
        /// </summary>
        public virtual ICollection<OrderNote> OrderNotes
        {
            get { return _orderNotes ?? (_orderNotes = new List<OrderNote>()); }
            protected set { _orderNotes = value; }
        }

        /// <summary>
        /// Gets or sets order product variants
        /// </summary>
        public virtual ICollection<OrderProductVariant> OrderProductVariants
        {
            get { return _orderProductVariant ?? (_orderProductVariant = new List<OrderProductVariant>()); }
            protected set { _orderProductVariant = value; }
        }
        #endregion

        #region Custom properties

        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        public virtual OrderStatus OrderStatus
        {
            get
            {
                return (OrderStatus)this.OrderStatusId;
            }
            set
            {
                this.OrderStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the payment status
        /// </summary>
        public virtual PaymentStatus PaymentStatus
        {
            get
            {
                return (PaymentStatus)this.PaymentStatusId;
            }
            set
            {
                this.PaymentStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the shipping status
        /// </summary>
        public virtual ShippingStatus ShippingStatus
        {
            get
            {
                return (ShippingStatus)this.ShippingStatusId;
            }
            set
            {
                this.ShippingStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the customer tax display type
        /// </summary>
        public virtual TaxDisplayType CustomerTaxDisplayType
        {
            get
            {
                return (TaxDisplayType)this.CustomerTaxDisplayTypeId;
            }
            set
            {
                this.CustomerTaxDisplayTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the applied tax rates
        /// </summary>
        public virtual SortedDictionary<decimal, decimal> TaxRatesDictionary
        {
            get
            {
                return ParseTaxRates(this.TaxRates);
            }
        }
        
        #endregion
    }
}
