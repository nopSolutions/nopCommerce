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
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;


namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Tax manager
    /// </summary>
    public partial class TaxManager
    {
        #region Utilities

        /// <summary>
        /// Gets a value indicating whether tax exempt
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>A value indicating whether tax exempt</returns>
        protected static bool IsTaxExempt(ProductVariant productVariant, Customer customer)
        {
            if (customer != null)
            {
                if (customer.IsTaxExempt)
                    return true;

                var customerRoles = customer.CustomerRoles;
                foreach (var customerRole in customerRoles)
                    if (customerRole.TaxExempt)
                        return true;
            }
            
            if (productVariant == null)
            {
                return false;
            }

            if (productVariant.IsTaxExempt)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether EU VAT exempt (the European Union Value Added Tax)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="customer">Customer</param>
        /// <returns>Result</returns>
        protected static bool IsVatExempt(Address address, Customer customer)
        {
            if (!TaxManager.EUVatEnabled)
            {
                return false;
            }

            if (address == null || address.Country == null || customer == null)
            {
                return false;
            }


            if (!address.Country.SubjectToVAT)
            {
                // VAT not chargeable if shipping outside VAT zone:
                return true;
            }
            else
            {
                // VAT not chargeable if address, customer and config meet our VAT exemption requirements:
                // returns true if this customer is VAT exempt because they are shipping within the EU but outside our shop country, they have supplied a validated VAT number, and the shop is configured to allow VAT exemption
                return address.CountryId != TaxManager.EUVatShopCountryId &&
                    customer.VatNumberStatus == VatNumberStatusEnum.Valid &&
                    TaxManager.EUVatAllowVATExemption;
            }
        }

        /// <summary>
        /// Performs a basic check of a VAT number for validity
        /// </summary>
        /// <remarks>Doesn't check the name and address</remarks>
        /// <returns>A value from the VatNumberStatusEnum enumeration</returns>
        protected static VatNumberStatusEnum DoVatCheck(string countryCode, string vatNumber, out string name, out string address, out Exception exception)
        {
            NopSolutions.NopCommerce.BusinessLogic.EuropaCheckVatService.checkVatService s = null;

            try
            {
                bool valid;
                vatNumber = vatNumber.Trim().Replace(" ", "");

                s = new NopSolutions.NopCommerce.BusinessLogic.EuropaCheckVatService.checkVatService();
                s.checkVat(ref countryCode, ref vatNumber, out valid, out name, out address);
                exception = null;
                return valid ? VatNumberStatusEnum.Valid : VatNumberStatusEnum.Invalid;
            }
            catch (Exception ex)
            {
                name = address = string.Empty;
                exception = ex;
                return VatNumberStatusEnum.Unknown;
            }
            finally
            {
                if (s != null)
                    s.Dispose();
            }
        }
        /// <summary>
        /// Create request for tax calculation
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="customer">Customer</param>
        /// <returns>Package for tax calculation</returns>
        protected static CalculateTaxRequest CreateCalculateTaxRequest(ProductVariant productVariant, 
            int taxClassId, Customer customer)
        {
            var calculateTaxRequest = new CalculateTaxRequest();
            calculateTaxRequest.Customer = customer;
            calculateTaxRequest.Item = productVariant;
            calculateTaxRequest.TaxClassId = taxClassId;

            var basedOn = TaxManager.TaxBasedOn;

            if (basedOn == TaxBasedOnEnum.BillingAddress)
            {
                if (customer == null || customer.BillingAddress == null)
                {
                    basedOn = TaxBasedOnEnum.DefaultAddress;
                }
            }
            if (basedOn == TaxBasedOnEnum.ShippingAddress)
            {
                if (customer == null || customer.ShippingAddress == null)
                {
                    basedOn = TaxBasedOnEnum.DefaultAddress;
                }
            }

            Address address = null;

            switch (basedOn)
            {
                case TaxBasedOnEnum.BillingAddress:
                    {
                        address = customer.BillingAddress;
                    }
                    break;
                case TaxBasedOnEnum.ShippingAddress:
                    {
                        address = customer.ShippingAddress;
                    }
                    break;
                case TaxBasedOnEnum.DefaultAddress:
                    {
                        address = TaxManager.DefaultTaxAddress;
                    }
                    break;
                case TaxBasedOnEnum.ShippingOrigin:
                    {
                        address = ShippingManager.ShippingOrigin;
                    }
                    break;
            }

            calculateTaxRequest.Address = address;
            return calculateTaxRequest;
        }

        /// <summary>
        /// Calculated price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="percent">Percent</param>
        /// <param name="increase">Increase</param>
        /// <returns>New price</returns>
        protected static decimal CalculatePrice(decimal price, decimal percent, bool increase)
        {
            decimal result = decimal.Zero;
            if (percent == decimal.Zero)
                return price;

            if (increase)
            {
                result = price * (1 + percent / 100);
            }
            else
            {
                result = price - (price) / (100 + percent) * percent;
            }
            return result;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax total</returns>
        public static decimal GetTaxTotal(ShoppingCart cart,
            Customer customer, ref string error)
        {
            return GetTaxTotal(cart, 0, customer, ref error);
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax total</returns>
        public static decimal GetTaxTotal(ShoppingCart cart, int paymentMethodId, 
            Customer customer, ref string error)
        {
            decimal taxTotal = decimal.Zero;

            //items
            decimal itemsTaxTotal = decimal.Zero;
            foreach (var shoppingCartItem in cart)
            {
                string error1 = string.Empty;
                string error2 = string.Empty;
                decimal subTotalWithoutDiscountExclTax = TaxManager.GetPrice(shoppingCartItem.ProductVariant, PriceHelper.GetSubTotal(shoppingCartItem, customer, true), false, customer, ref error1);
                decimal subTotalWithoutDiscountInclTax = TaxManager.GetPrice(shoppingCartItem.ProductVariant, PriceHelper.GetSubTotal(shoppingCartItem, customer, true), true, customer, ref error2);
                if (!String.IsNullOrEmpty(error1))
                {
                    error = error1;
                }
                if (!String.IsNullOrEmpty(error2))
                {
                    error = error2;
                }

                decimal shoppingCartItemTax = subTotalWithoutDiscountInclTax - subTotalWithoutDiscountExclTax;
                itemsTaxTotal += shoppingCartItemTax;
            }

            //checkout attributes
            decimal checkoutAttributesTax = decimal.Zero;
            if (customer != null)
            {
                var caValues = CheckoutAttributeHelper.ParseCheckoutAttributeValues(customer.CheckoutAttributes);
                foreach (var caValue in caValues)
                {
                    string error1 = string.Empty;
                    string error2 = string.Empty;
                    decimal caExclTax = TaxManager.GetCheckoutAttributePrice(caValue, false, customer, ref error1);
                    decimal caInclTax = TaxManager.GetCheckoutAttributePrice(caValue, true, customer, ref error2);
                    if (!String.IsNullOrEmpty(error1))
                    {
                        error = error1;
                    }
                    if (!String.IsNullOrEmpty(error2))
                    {
                        error = error2;
                    }

                    decimal caTax = caInclTax - caExclTax;
                    checkoutAttributesTax += caTax;
                }
            }

            //shipping
            decimal shippingTax = decimal.Zero;
            if (TaxManager.ShippingIsTaxable)
            {
                string error1 = string.Empty;
                string error2 = string.Empty;
                decimal? shippingExclTax = ShippingManager.GetShoppingCartShippingTotal(cart, customer, false, ref error1);
                decimal? shippingInclTax = ShippingManager.GetShoppingCartShippingTotal(cart, customer, true, ref error2);
                if (!String.IsNullOrEmpty(error1))
                {
                    error = error1;
                }
                if (!String.IsNullOrEmpty(error2))
                {
                    error = error2;
                }
                if (shippingExclTax.HasValue && shippingInclTax.HasValue)
                {
                    shippingTax = shippingInclTax.Value - shippingExclTax.Value;
                }
            }

            //payment method additional fee
            decimal paymentMethodAdditionalFeeTax = decimal.Zero;
            if (TaxManager.PaymentMethodAdditionalFeeIsTaxable)
            {
                string error1 = string.Empty;
                string error2 = string.Empty;
                decimal paymentMethodAdditionalFee = PaymentManager.GetAdditionalHandlingFee(paymentMethodId);
                decimal? paymentMethodAdditionalFeeExclTax = TaxManager.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, false, customer, ref error1);
                decimal? paymentMethodAdditionalFeeInclTax = TaxManager.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, true, customer, ref error2);
                if (!String.IsNullOrEmpty(error1))
                {
                    error = error1;
                }
                if (!String.IsNullOrEmpty(error2))
                {
                    error = error2;
                }
                if (paymentMethodAdditionalFeeExclTax.HasValue && paymentMethodAdditionalFeeInclTax.HasValue)
                {
                    paymentMethodAdditionalFeeTax = paymentMethodAdditionalFeeInclTax.Value - paymentMethodAdditionalFeeExclTax.Value;
                }
            }

            taxTotal = itemsTaxTotal + checkoutAttributesTax + shippingTax + paymentMethodAdditionalFeeTax;
            taxTotal = Math.Round(taxTotal, 2);
            return taxTotal;
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        public static decimal GetTaxRate(ProductVariant productVariant, 
            Customer customer, ref string error)
        {
            return GetTaxRate(productVariant, 0, customer, ref error);
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        public static decimal GetTaxRate(int taxClassId, Customer customer, ref string error)
        {
            return GetTaxRate(null, taxClassId, customer, ref error);
        }
        
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Tax rate</returns>
        public static decimal GetTaxRate(ProductVariant productVariant, 
            int taxClassId, Customer customer, ref string error)
        {
            //tax exempt
            if (IsTaxExempt(productVariant, customer))
            {
                return decimal.Zero;
            }

            //tax request
            var calculateTaxRequest = CreateCalculateTaxRequest(productVariant, taxClassId, customer);

            //make EU VAT exempt validation (the European Union Value Added Tax)
            if (TaxManager.EUVatEnabled)
            {
                if (IsVatExempt(calculateTaxRequest.Address, calculateTaxRequest.Customer))
                {
                    //return zero if VAT is not chargeable
                    return decimal.Zero;
                }
            }

            //instantiate tax provider
            var activeTaxProvider = TaxManager.ActiveTaxProvider;
            if (activeTaxProvider == null)
                throw new NopException("Tax provider could not be loaded");
            var iTaxProvider = Activator.CreateInstance(Type.GetType(activeTaxProvider.ClassName)) as ITaxProvider;

            //get tax rate
            decimal taxRate = iTaxProvider.GetTaxRate(calculateTaxRequest, ref error);
            return taxRate;
        }
        
        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <returns>Price</returns>
        public static decimal GetPrice(ProductVariant productVariant, decimal price)
        {
            string error = string.Empty;
            return GetPrice(productVariant, price, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetPrice(ProductVariant productVariant, 
            decimal price, ref string error)
        {
            var customer = NopContext.Current.User;
            return GetPrice(productVariant, price, customer, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public static decimal GetPrice(ProductVariant productVariant, decimal price, Customer customer)
        {
            string error = string.Empty;
            return GetPrice(productVariant, price, customer, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetPrice(ProductVariant productVariant, decimal price,
            Customer customer, ref string error)
        {
            bool includingTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetPrice(productVariant, price, includingTax, customer, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public static decimal GetPrice(ProductVariant productVariant, decimal price,
            bool includingTax, Customer customer)
        {
            string error = string.Empty;
            return GetPrice(productVariant, price, includingTax, customer, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetPrice(ProductVariant productVariant, decimal price,
            bool includingTax, Customer customer, ref string error)
        {
            bool priceIncludesTax = TaxManager.PricesIncludeTax;
            int taxClassId = 0;
            return GetPrice(productVariant, taxClassId, price, includingTax, customer, priceIncludesTax, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <returns>Price</returns>
        public static decimal GetPrice(ProductVariant productVariant, int taxClassId, 
            decimal price, bool includingTax, Customer customer, bool priceIncludesTax)
        {
            string error = string.Empty;
            return GetPrice(productVariant, taxClassId, price, includingTax, customer, priceIncludesTax, ref error);           
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="taxClassId">Tax class identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetPrice(ProductVariant productVariant, int taxClassId, 
            decimal price,  bool includingTax, Customer customer, 
            bool priceIncludesTax, ref string error)
        {
            if (priceIncludesTax)
            {
                if (!includingTax)
                {
                    decimal includingPercent = GetTaxRate(productVariant, taxClassId, customer, ref error);
                    price = CalculatePrice(price, includingPercent, false);
                }
            }
            else
            {
                if (includingTax)
                {
                    decimal percent = GetTaxRate(productVariant, taxClassId, customer, ref error);
                    price = CalculatePrice(price, percent, true);
                }
            }

            if (price < decimal.Zero)
                price = decimal.Zero;
            price = Math.Round(price, 2);

            return price;
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public static decimal GetShippingPrice(decimal price, Customer customer)
        {
            string error = string.Empty;
            return GetShippingPrice(price, customer, ref error);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetShippingPrice(decimal price,  Customer customer, 
            ref string error)
        {
            bool includingTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetShippingPrice(price, includingTax, customer, ref error);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public static decimal GetShippingPrice(decimal price, bool includingTax, Customer customer)
        {
            string error = string.Empty;
            return GetShippingPrice(price, includingTax, customer, ref error);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetShippingPrice(decimal price, bool includingTax, 
            Customer customer, ref string error)
        {
            if (!TaxManager.ShippingIsTaxable)
            {
                return price;
            }
            int taxClassId = TaxManager.ShippingTaxClassId;
            bool priceIncludesTax = TaxManager.ShippingPriceIncludesTax;
            return GetPrice(null, taxClassId, price, includingTax, customer, priceIncludesTax, ref error);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public static decimal GetPaymentMethodAdditionalFee(decimal price, Customer customer)
        {
            string error = string.Empty;
            return GetPaymentMethodAdditionalFee(price, customer, ref error);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetPaymentMethodAdditionalFee(decimal price, 
            Customer customer, ref string error)
        {
            bool includingTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetPaymentMethodAdditionalFee(price, includingTax, customer, ref error);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public static decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, Customer customer)
        {
            string error = string.Empty;
            return GetPaymentMethodAdditionalFee(price, includingTax, customer, ref error);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetPaymentMethodAdditionalFee(decimal price, 
            bool includingTax, Customer customer, ref string error)
        {
            if (!TaxManager.PaymentMethodAdditionalFeeIsTaxable)
            {
                return price;
            }
            int taxClassId = TaxManager.PaymentMethodAdditionalFeeTaxClassId;
            bool priceIncludesTax = TaxManager.PaymentMethodAdditionalFeeIncludesTax;
            return GetPrice(null, taxClassId, price, includingTax, customer, priceIncludesTax, ref error);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>Price</returns>
        public static decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav)
        {
            var customer = NopContext.Current.User;
            return GetCheckoutAttributePrice(cav, customer);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public static decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, 
            Customer customer)
        {
            string error = string.Empty;
            return GetCheckoutAttributePrice(cav, customer, ref error);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, 
            Customer customer, ref string error)
        {
            bool includingTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    includingTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    includingTax = true;
                    break;
            }
            return GetCheckoutAttributePrice(cav,
                includingTax, customer, ref error);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public static decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, 
            bool includingTax, Customer customer)
        {
            string error = string.Empty;
            return GetCheckoutAttributePrice(cav, 
                includingTax, customer, ref error);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public static decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, Customer customer, ref string error)
        {
            if (cav == null)
                throw new ArgumentNullException("cav");

            decimal price = cav.PriceAdjustment;
            if (cav.CheckoutAttribute.IsTaxExempt)
            {
                return price;
            }

            bool priceIncludesTax = TaxManager.PricesIncludeTax;
            int taxClassId = cav.CheckoutAttribute.TaxCategoryId;
            return GetPrice(null, taxClassId, price, includingTax, customer, priceIncludesTax, ref error);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>VAT Number status</returns>
        public static VatNumberStatusEnum GetVatNumberStatus(Country country,
            string vatNumber)
        {
            if (vatNumber == null)
                vatNumber = string.Empty;

            vatNumber = vatNumber.Trim();

            if (String.IsNullOrEmpty(vatNumber))
                return VatNumberStatusEnum.Empty;

            if (country == null)
                return VatNumberStatusEnum.Unknown;

            if (!TaxManager.EUVatUseWebService)
                return VatNumberStatusEnum.Unknown;


            //UNDONE
            try
            {
                string name = string.Empty;
                string address = string.Empty;
                Exception exception = null;
                return DoVatCheck(country.TwoLetterIsoCode, vatNumber, out name, out address, out exception);
            }
            catch (Exception exc)
            {
                return VatNumberStatusEnum.Unknown;
            }

        }

        /// <summary>
        /// Gets VAT Number status name
        /// </summary>
        /// <param name="status">VAT Number status</param>
        /// <returns>VAT Number status name</returns>
        public static string GetVatNumberStatusName(VatNumberStatusEnum status)
        {
            return LocalizationManager.GetLocaleResourceString(string.Format("VatNumberStatus.{0}", status.ToString()));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Tax based on
        /// </summary>
        public static TaxBasedOnEnum TaxBasedOn
        {
            get
            {
                int taxBasedOn = SettingManager.GetSettingValueInteger("Tax.TaxBasedOn");
                return (TaxBasedOnEnum)taxBasedOn;
            }
            set
            {
                SettingManager.SetParam("Tax.TaxBasedOn", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Tax display type
        /// </summary>
        public static TaxDisplayTypeEnum TaxDisplayType
        {
            get
            {
                int taxBasedOn = SettingManager.GetSettingValueInteger("Tax.TaxDisplayType");
                return (TaxDisplayTypeEnum)taxBasedOn;
            }
            set
            {
                SettingManager.SetParam("Tax.TaxDisplayType", ((int)value).ToString());
            }
        }
        
        /// <summary>
        /// Gets or sets an active shipping rate computation method
        /// </summary>
        public static TaxProvider ActiveTaxProvider
        {
            get
            {
                int taxProviderId = SettingManager.GetSettingValueInteger("Tax.TaxProvider.ActiveId");
                return TaxProviderManager.GetTaxProviderById(taxProviderId);
            }
            set
            {
                if (value != null)
                    SettingManager.SetParam("Tax.TaxProvider.ActiveId", value.TaxProviderId.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a default tax address
        /// </summary>
        public static Address DefaultTaxAddress
        {
            get
            {
                int countryId = SettingManager.GetSettingValueInteger("Tax.DefaultTaxAddress.CountryId");
                int stateProvinceId = SettingManager.GetSettingValueInteger("Tax.DefaultTaxAddress.StateProvinceId");
                string zipPostalCode = SettingManager.GetSettingValue("Tax.DefaultTaxAddress.ZipPostalCode");
                Address address = new Address();
                address.CountryId = countryId;
                address.StateProvinceId = stateProvinceId;
                address.ZipPostalCode = zipPostalCode;
                return address;
            }
            set
            {
                int countryId = 0;
                int stateProvinceId = 0;
                string zipPostalCode = string.Empty;

                if (value != null)
                {
                    countryId = value.CountryId;
                    stateProvinceId = value.StateProvinceId;
                    zipPostalCode = value.ZipPostalCode;
                }

                SettingManager.SetParam("Tax.DefaultTaxAddress.CountryId", countryId.ToString());
                SettingManager.SetParam("Tax.DefaultTaxAddress.StateProvinceId", stateProvinceId.ToString());
                SettingManager.SetParam("Tax.DefaultTaxAddress.ZipPostalCode", zipPostalCode);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display tax suffix
        /// </summary>
        public static bool DisplayTaxSuffix
        {
            get
            {
                bool displayTaxSuffix = SettingManager.GetSettingValueBoolean("Tax.DisplayTaxSuffix");
                return displayTaxSuffix;
            }
            set
            {
                SettingManager.SetParam("Tax.DisplayTaxSuffix", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether prices incude tax
        /// </summary>
        public static bool PricesIncludeTax
        {
            get
            {
                bool pricesIncludeTax = SettingManager.GetSettingValueBoolean("Tax.PricesIncludeTax");
                return pricesIncludeTax;
            }
            set
            {
                SettingManager.SetParam("Tax.PricesIncludeTax", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select tax display type
        /// </summary>
        public static bool AllowCustomersToSelectTaxDisplayType
        {
            get
            {
                bool allowCustomersToSelectTaxDisplayType = SettingManager.GetSettingValueBoolean("Tax.AllowCustomersToSelectTaxDisplayType");
                return allowCustomersToSelectTaxDisplayType;
            }
            set
            {
                SettingManager.SetParam("Tax.AllowCustomersToSelectTaxDisplayType", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide zero tax
        /// </summary>
        public static bool HideZeroTax
        {
            get
            {
                bool hideZeroTax = SettingManager.GetSettingValueBoolean("Tax.HideZeroTax");
                return hideZeroTax;
            }
            set
            {
                SettingManager.SetParam("Tax.HideZeroTax", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide tax in order summary when prices are shown tax inclusive
        /// </summary>
        public static bool HideTaxInOrderSummary
        {
            get
            {
                bool hideTaxInOrderSummary = SettingManager.GetSettingValueBoolean("Tax.HideTaxInOrderSummary");
                return hideTaxInOrderSummary;
            }
            set
            {
                SettingManager.SetParam("Tax.HideTaxInOrderSummary", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether shipping price is taxable
        /// </summary>
        public static bool ShippingIsTaxable
        {
            get
            {
                bool shippingIsTaxable = SettingManager.GetSettingValueBoolean("Tax.ShippingIsTaxable");
                return shippingIsTaxable;
            }
            set
            {
                SettingManager.SetParam("Tax.ShippingIsTaxable", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether shipping price incudes tax
        /// </summary>
        public static bool ShippingPriceIncludesTax
        {
            get
            {
                bool shippingPriceIncludesTax = SettingManager.GetSettingValueBoolean("Tax.ShippingPriceIncludesTax");
                return shippingPriceIncludesTax;
            }
            set
            {
                SettingManager.SetParam("Tax.ShippingPriceIncludesTax", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the shipping tax class identifier
        /// </summary>
        public static int ShippingTaxClassId
        {
            get
            {
                int shippingTaxClassId = SettingManager.GetSettingValueInteger("Tax.ShippingTaxClassId");
                return shippingTaxClassId;
            }
            set
            {
                SettingManager.SetParam("Tax.ShippingTaxClassId", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether payment method additional fee is taxable
        /// </summary>
        public static bool PaymentMethodAdditionalFeeIsTaxable
        {
            get
            {
                bool paymentMethodAdditionalFeeIsTaxable = SettingManager.GetSettingValueBoolean("Tax.PaymentMethodAdditionalFeeIsTaxable");
                return paymentMethodAdditionalFeeIsTaxable;
            }
            set
            {
                SettingManager.SetParam("Tax.PaymentMethodAdditionalFeeIsTaxable", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether payment method additional fee incudes tax
        /// </summary>
        public static bool PaymentMethodAdditionalFeeIncludesTax
        {
            get
            {
                bool paymentMethodAdditionalFeeIncludesTax = SettingManager.GetSettingValueBoolean("Tax.PaymentMethodAdditionalFeeIncludesTax");
                return paymentMethodAdditionalFeeIncludesTax;
            }
            set
            {
                SettingManager.SetParam("Tax.PaymentMethodAdditionalFeeIncludesTax", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the payment method additional fee tax class identifier
        /// </summary>
        public static int PaymentMethodAdditionalFeeTaxClassId
        {
            get
            {
                int paymentMethodAdditionalFeeTaxClassId = SettingManager.GetSettingValueInteger("Tax.PaymentMethodAdditionalFeeTaxClassId");
                return paymentMethodAdditionalFeeTaxClassId;
            }
            set
            {
                SettingManager.SetParam("Tax.PaymentMethodAdditionalFeeTaxClassId", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EU VAT (Eupore Union Value Added Tax) is enabled
        /// </summary>
        public static bool EUVatEnabled
        {
            get
            {
                bool result = SettingManager.GetSettingValueBoolean("Tax.EUVat.Enabled");
                return result;
            }
            set
            {
                SettingManager.SetParam("Tax.EUVat.Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a shop country identifier
        /// </summary>
        public static int EUVatShopCountryId
        {
            get
            {
                int result = SettingManager.GetSettingValueInteger("Tax.EUVat.EUVatShopCountryId");
                return result;
            }
            set
            {
                SettingManager.SetParam("Tax.EUVat.EUVatShopCountryId", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this store will exempt eligible VAT-registered customers from VAT
        /// </summary>
        public static bool EUVatAllowVATExemption
        {
            get
            {
                bool result = SettingManager.GetSettingValueBoolean("Tax.EUVat.AllowVATExemption", true);
                return result;
            }
            set
            {
                SettingManager.SetParam("Tax.EUVat.AllowVATExemption", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we should use the EU web service to validate VAT numbers
        /// </summary>
        public static bool EUVatUseWebService
        {
            get
            {
                bool result = SettingManager.GetSettingValueBoolean("Tax.EUVat.UseWebService");
                return result;
            }
            set
            {
                SettingManager.SetParam("Tax.EUVat.UseWebService", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we should notify a store owner when a new VAT number is submitted
        /// </summary>
        public static bool EUVatEmailAdminWhenNewVATSubmitted
        {
            get
            {
                bool result = SettingManager.GetSettingValueBoolean("Tax.EUVat.EUVatEmailAdminWhenNewVATSubmitted");
                return result;
            }
            set
            {
                SettingManager.SetParam("Tax.EUVat.EUVatEmailAdminWhenNewVATSubmitted", value.ToString());
            }
        }

        #endregion
    }
}
