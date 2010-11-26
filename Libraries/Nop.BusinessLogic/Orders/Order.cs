//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;

namespace NopSolutions.NopCommerce.BusinessLogic.Orders
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public partial class Order : BaseEntity
    {
        #region Fields
        private Customer _customer = null;
        private RewardPointsHistory _rph = null;
        #endregion

        #region Utilities

        protected SortedDictionary<decimal, decimal> ParseTaxRates(string taxRatesStr)
        {
            var taxRatesDictionary = new SortedDictionary<decimal, decimal>();
            if (String.IsNullOrEmpty(taxRatesStr))
                return taxRatesDictionary;

            string[] lines = taxRatesStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (String.IsNullOrEmpty(line.Trim()))
                    continue;

                string[] taxes = line.Split(new char[] { ':'});
                if (taxes.Length == 2)
                {
                    try
                    {
                        decimal taxRate = decimal.Parse(taxes[0].Trim(), new CultureInfo("en-US"));
                        decimal taxValue = decimal.Parse(taxes[1].Trim(), new CultureInfo("en-US"));
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
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public Guid OrderGuid { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer language identifier
        /// </summary>
        public int CustomerLanguageId { get; set; }

        /// <summary>
        /// Gets or sets the customer tax display type identifier
        /// </summary>
        public int CustomerTaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer IP
        /// </summary>
        public string CustomerIP { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (incl tax)
        /// </summary>
        public decimal OrderSubtotalInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (excl tax)
        /// </summary>
        public decimal OrderSubtotalExclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount (incl tax)
        /// </summary>
        public decimal OrderSubTotalDiscountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount (excl tax)
        /// </summary>
        public decimal OrderSubTotalDiscountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping (incl tax)
        /// </summary>
        public decimal OrderShippingInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping (excl tax)
        /// </summary>
        public decimal OrderShippingExclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee (incl tax)
        /// </summary>
        public decimal PaymentMethodAdditionalFeeInclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee (excl tax)
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
        /// Gets or sets the order total
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the refunded amount
        /// </summary>
        public decimal RefundedAmount { get; set; }

        /// <summary>
        /// Gets or sets the order discount (applied to order total)
        /// </summary>
        public decimal OrderDiscount { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal incl tax (customer currency)
        /// </summary>
        public decimal OrderSubtotalInclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal excl tax (customer currency)
        /// </summary>
        public decimal OrderSubtotalExclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount incl tax (customer currency)
        /// </summary>
        public decimal OrderSubTotalDiscountInclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount excl tax (customer currency)
        /// </summary>
        public decimal OrderSubTotalDiscountExclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the order shipping incl tax (customer currency)
        /// </summary>
        public decimal OrderShippingInclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the order shipping excl tax (customer currency)
        /// </summary>
        public decimal OrderShippingExclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee incl tax (customer currency)
        /// </summary>
        public decimal PaymentMethodAdditionalFeeInclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee excl tax (customer currency)
        /// </summary>
        public decimal PaymentMethodAdditionalFeeExclTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the tax rates (customer currency)
        /// </summary>
        public string TaxRatesInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the order tax (customer currency)
        /// </summary>
        public decimal OrderTaxInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the order total (customer currency)
        /// </summary>
        public decimal OrderTotalInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the order discount (customer currency) (applied to order total)
        /// </summary>
        public decimal OrderDiscountInCustomerCurrency { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute description
        /// </summary>
        public string CheckoutAttributeDescription { get; set; }

        /// <summary>
        /// Gets or sets the checkout attributes in XML format
        /// </summary>
        public string CheckoutAttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the customer currency code
        /// </summary>
        public string CustomerCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the order weight
        /// </summary>
        public decimal OrderWeight { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public int AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets an order status identifer
        /// </summary>
        public int OrderStatusId { get; set; }

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
        /// Gets or sets the payment method identifier
        /// </summary>
        public int PaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets the payment method name
        /// </summary>
        public string PaymentMethodName { get; set; }

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
        /// Gets or sets the purchase order number
        /// </summary>
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the payment status identifier
        /// </summary>
        public int PaymentStatusId { get; set; }

        /// <summary>
        /// Gets or sets the paid date and time
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// Gets or sets the billing first name
        /// </summary>
        public string BillingFirstName { get; set; }

        /// <summary>
        /// Gets or sets the billing last name
        /// </summary>
        public string BillingLastName { get; set; }

        /// <summary>
        /// Gets or sets the billing phone number
        /// </summary>
        public string BillingPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the billing email
        /// </summary>
        public string BillingEmail { get; set; }

        /// <summary>
        /// Gets or sets the billing fax number
        /// </summary>
        public string BillingFaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the billing company
        /// </summary>
        public string BillingCompany { get; set; }

        /// <summary>
        /// Gets or sets the billing address 1
        /// </summary>
        public string BillingAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the billing address 2
        /// </summary>
        public string BillingAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the billing city
        /// </summary>
        public string BillingCity { get; set; }

        /// <summary>
        /// Gets or sets the billing state/province
        /// </summary>
        public string BillingStateProvince { get; set; }

        /// <summary>
        /// Gets or sets the billing state/province identifier
        /// </summary>
        public int BillingStateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the billing zip/postal code
        /// </summary>
        public string BillingZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the billing country
        /// </summary>
        public string BillingCountry { get; set; }

        /// <summary>
        /// Gets or sets the billing country identifier
        /// </summary>
        public int BillingCountryId { get; set; }

        /// <summary>
        /// Gets or sets the shipping status identifier
        /// </summary>
        public int ShippingStatusId { get; set; }

        /// <summary>
        /// Gets or sets the shipping first name
        /// </summary>
        public string ShippingFirstName { get; set; }

        /// <summary>
        /// Gets or sets the shipping last name
        /// </summary>
        public string ShippingLastName { get; set; }

        /// <summary>
        /// Gets or sets the shipping phone number
        /// </summary>
        public string ShippingPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the shipping email
        /// </summary>
        public string ShippingEmail { get; set; }

        /// <summary>
        /// Gets or sets the shipping fax number
        /// </summary>
        public string ShippingFaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the shipping  company
        /// </summary>
        public string ShippingCompany { get; set; }

        /// <summary>
        /// Gets or sets the shipping address 1
        /// </summary>
        public string ShippingAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the shipping address 2
        /// </summary>
        public string ShippingAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the shipping city
        /// </summary>
        public string ShippingCity { get; set; }

        /// <summary>
        /// Gets or sets the shipping state/province
        /// </summary>
        public string ShippingStateProvince { get; set; }

        /// <summary>
        /// Gets or sets the shipping state/province identifier
        /// </summary>
        public int ShippingStateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the shipping zip/postal code
        /// </summary>
        public string ShippingZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the shipping country
        /// </summary>
        public string ShippingCountry { get; set; }

        /// <summary>
        /// Gets or sets the shipping identifier
        /// </summary>
        public int ShippingCountryId { get; set; }

        /// <summary>
        /// Gets or sets the shipping method
        /// </summary>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// Gets or sets the shipping rate computation method identifier
        /// </summary>
        public int ShippingRateComputationMethodId { get; set; }

        /// <summary>
        /// Gets or sets the shipped date and time
        /// </summary>
        public DateTime? ShippedDate { get; set; }

        /// <summary>
        /// Gets or sets the delivery date and time
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets the tracking number of current order
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets the VAT number (the European Union Value Added Tax)
        /// </summary>
        public string VatNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the customer
        /// </summary>
        public Customer Customer
        {
            get
            {
                if (_customer == null)
                    _customer = IoC.Resolve<ICustomerService>().GetCustomerById(this.CustomerId);
                return _customer;
            }
        }

        /// <summary>
        /// Gets or sets the customer tax display type
        /// </summary>
        public TaxDisplayTypeEnum CustomerTaxDisplayType
        {
            get
            {
                return (TaxDisplayTypeEnum)this.CustomerTaxDisplayTypeId;
            }
            set
            {
                this.CustomerTaxDisplayTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the affiliate
        /// </summary>
        public Affiliate Affiliate
        {
            get
            {
                return IoC.Resolve<IAffiliateService>().GetAffiliateById(this.AffiliateId);
            }
        }

        /// <summary>
        /// Gets the order product variants
        /// </summary>
        public List<OrderProductVariant> OrderProductVariants
        {
            get
            {
                return IoC.Resolve<IOrderService>().GetOrderProductVariantsByOrderId(this.OrderId);
            }
        }

        /// <summary>
        /// Gets the order notes
        /// </summary>
        public List<OrderNote> OrderNotes
        {
            get
            {
                return IoC.Resolve<IOrderService>().GetOrderNoteByOrderId(this.OrderId);
            }
        }

        /// <summary>
        /// Gets or sets the order status
        /// </summary>
        public OrderStatusEnum OrderStatus
        {
            get
            {
                return (OrderStatusEnum)this.OrderStatusId;
            }
            set
            {
                this.OrderStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the payment status
        /// </summary>
        public PaymentStatusEnum PaymentStatus
        {
            get
            {
                return (PaymentStatusEnum)this.PaymentStatusId;
            }
            set
            {
                this.PaymentStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the shipping status
        /// </summary>
        public ShippingStatusEnum ShippingStatus
        {
            get
            {
                return (ShippingStatusEnum)this.ShippingStatusId;
            }
            set
            {
                this.ShippingStatusId = (int)value;
            }
        }
        
        /// <summary>
        /// Gets the billing full name
        /// </summary>
        public string BillingFullName
        {
            get
            {
                if (String.IsNullOrEmpty(this.BillingFirstName))
                    return this.BillingLastName;
                else
                    return string.Format("{0} {1}", this.BillingFirstName, this.BillingLastName);
            }
        }

        /// <summary>
        /// Gets the shipping full name
        /// </summary>
        public string ShippingFullName
        {
            get
            {
                if (String.IsNullOrEmpty(this.ShippingFirstName))
                    return this.ShippingLastName;
                else
                    return string.Format("{0} {1}", this.ShippingFirstName, this.ShippingLastName);
            }
        }

        /// <summary>
        /// Gets a redeemed reward points history entry
        /// </summary>
        public RewardPointsHistory RedeemedRewardPoints
        {
            get
            {
                if (_rph == null)
                {
                    if (this.CustomerId == 0)
                        return null;

                    var rphc = IoC.Resolve<IOrderService>().GetAllRewardPointsHistoryEntries(this.CustomerId,
                        this.OrderId, 0, 1);
                    if (rphc.Count > 0)
                    {
                        _rph = rphc[0];
                    }
                }
                return _rph;
            }
        }

        /// <summary>
        /// Gets the applied tax rates
        /// </summary>
        public SortedDictionary<decimal, decimal> TaxRatesDictionary
        {
            get
            {
                return ParseTaxRates(this.TaxRates);
            }
        }

        /// <summary>
        /// Gets the applied tax rates (customer currency)
        /// </summary>
        public SortedDictionary<decimal, decimal> TaxRatesDictionaryInCustomerCurrency
        {
            get
            {
                return ParseTaxRates(this.TaxRatesInCustomerCurrency);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the order notes
        /// </summary>
        public virtual ICollection<OrderNote> NpOrderNotes { get; set; }

        /// <summary>
        /// Gets the order product variants
        /// </summary>
        public virtual ICollection<OrderProductVariant> NpOrderProductVariants { get; set; }

        /// <summary>
        /// Gets the discount usage history
        /// </summary>
        public virtual ICollection<DiscountUsageHistory> NpDiscountUsageHistory { get; set; }
        
        #endregion
    }
}
