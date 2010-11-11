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
// Contributor(s): Stephen Kennedy - VAT support, _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial interface ITaxService
    {
        #region Methods

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax total</returns>
        decimal GetTaxTotal(ShoppingCart cart,
            Customer customer, ref string error);

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax total</returns>
        decimal GetTaxTotal(ShoppingCart cart, int paymentMethodId,
            Customer customer, ref string error);

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRates">Tax rates</param>
        /// <param name="error">Error</param>
        /// <returns>Tax total</returns>
        decimal GetTaxTotal(ShoppingCart cart, int paymentMethodId,
            Customer customer, out SortedDictionary<decimal, decimal> taxRates, ref string error);

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(ProductVariant productVariant,
            Customer customer, ref string error);

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(int taxClassId, Customer customer, ref string error);
        
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        decimal GetTaxRate(ProductVariant productVariant,
            int taxClassId, Customer customer, ref string error);
        
        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetPrice(ProductVariant productVariant, decimal price,
            out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetPrice(ProductVariant productVariant, decimal price,
            out decimal taxRate, ref string error);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetPrice(ProductVariant productVariant, decimal price,
            Customer customer, out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetPrice(ProductVariant productVariant, decimal price,
            Customer customer, out decimal taxRate, ref string error);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetPrice(ProductVariant productVariant, decimal price,
            bool includingTax, Customer customer, out decimal taxRate);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetPrice(ProductVariant productVariant, decimal price,
            bool includingTax, Customer customer, out decimal taxRate, ref string error);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        decimal GetPrice(ProductVariant productVariant, int taxClassId,
            decimal price, bool includingTax, Customer customer, out decimal taxRate,
            bool priceIncludesTax);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetPrice(ProductVariant productVariant, int taxClassId,
            decimal price, bool includingTax, Customer customer,
            bool priceIncludesTax, out decimal taxRate, ref string error);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        decimal GetShippingPrice(decimal price, Customer customer);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetShippingPrice(decimal price, Customer customer,
            ref string error);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        decimal GetShippingPrice(decimal price, bool includingTax, Customer customer);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetShippingPrice(decimal price, bool includingTax,
            Customer customer, ref string error);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetShippingPrice(decimal price, bool includingTax,
            Customer customer, out decimal taxRate, ref string error);

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        decimal GetPaymentMethodAdditionalFee(decimal price, Customer customer);

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetPaymentMethodAdditionalFee(decimal price,
            Customer customer, ref string error);

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, Customer customer);

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetPaymentMethodAdditionalFee(decimal price,
            bool includingTax, Customer customer, ref string error);
        
        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetPaymentMethodAdditionalFee(decimal price,
            bool includingTax, Customer customer, out decimal taxRate, ref string error);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            Customer customer);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            Customer customer, ref string error);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, Customer customer);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, Customer customer, ref string error);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, Customer customer, out decimal taxRate, ref string error);

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>VAT Number status</returns>
        VatNumberStatusEnum GetVatNumberStatus(Country country,
            string vatNumber);
        
        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        VatNumberStatusEnum GetVatNumberStatus(Country country,
            string vatNumber, out string name, out string address);

        /// <summary>
        /// Gets VAT Number status name
        /// </summary>
        /// <param name="status">VAT Number status</param>
        /// <returns>VAT Number status name</returns>
        string GetVatNumberStatusName(VatNumberStatusEnum status);

        #endregion

        #region Properties

        /// <summary>
        /// Tax based on
        /// </summary>
        TaxBasedOnEnum TaxBasedOn { get; set; }

        /// <summary>
        /// Tax display type
        /// </summary>
        TaxDisplayTypeEnum TaxDisplayType { get; set; }
        
        /// <summary>
        /// Gets or sets an active shipping rate computation method
        /// </summary>
        TaxProvider ActiveTaxProvider { get; set; }

        /// <summary>
        /// Gets or sets a default tax address
        /// </summary>
        Address DefaultTaxAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to display tax suffix
        /// </summary>
        bool DisplayTaxSuffix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether each tax rate should be displayed on separate line (shopping cart page)
        /// </summary>
        bool DisplayTaxRates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether prices incude tax
        /// </summary>
        bool PricesIncludeTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select tax display type
        /// </summary>
        bool AllowCustomersToSelectTaxDisplayType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide zero tax
        /// </summary>
        bool HideZeroTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to hide tax in order summary when prices are shown tax inclusive
        /// </summary>
        bool HideTaxInOrderSummary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shipping price is taxable
        /// </summary>
        bool ShippingIsTaxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shipping price incudes tax
        /// </summary>
        bool ShippingPriceIncludesTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the shipping tax class identifier
        /// </summary>
        int ShippingTaxClassId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether payment method additional fee is taxable
        /// </summary>
        bool PaymentMethodAdditionalFeeIsTaxable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether payment method additional fee incudes tax
        /// </summary>
        bool PaymentMethodAdditionalFeeIncludesTax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the payment method additional fee tax class identifier
        /// </summary>
        int PaymentMethodAdditionalFeeTaxClassId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether EU VAT (Eupore Union Value Added Tax) is enabled
        /// </summary>
        bool EUVatEnabled { get; set; }

        /// <summary>
        /// Gets or sets a shop country identifier
        /// </summary>
        int EUVatShopCountryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this store will exempt eligible VAT-registered customers from VAT
        /// </summary>
        bool EUVatAllowVATExemption { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should use the EU web service to validate VAT numbers
        /// </summary>
        bool EUVatUseWebService { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should notify a store owner when a new VAT number is submitted
        /// </summary>
        bool EUVatEmailAdminWhenNewVATSubmitted { get; set; }

        #endregion
    }
}
