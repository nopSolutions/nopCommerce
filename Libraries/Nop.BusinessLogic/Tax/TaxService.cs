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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;


namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial class TaxService : ITaxService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public TaxService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a value indicating whether tax exempt
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>A value indicating whether tax exempt</returns>
        protected bool IsTaxExempt(ProductVariant productVariant, Customer customer)
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
        protected bool IsVatExempt(Address address, Customer customer)
        {
            if (!this.EUVatEnabled)
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
                return address.CountryId != this.EUVatShopCountryId &&
                    customer.VatNumberStatus == VatNumberStatusEnum.Valid &&
                    this.EUVatAllowVATExemption;
            }
        }

        /// <summary>
        /// Performs a basic check of a VAT number for validity
        /// </summary>
        /// <remarks>Doesn't check the name and address</remarks>
        /// <returns>A value from the VatNumberStatusEnum enumeration</returns>
        protected VatNumberStatusEnum DoVatCheck(string countryCode, string vatNumber, out string name, out string address, out Exception exception)
        {
            name = string.Empty;
            address = string.Empty;

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
                if (name == null)
                    name = string.Empty;

                if (address == null)
                    address = string.Empty;

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
        protected CalculateTaxRequest CreateCalculateTaxRequest(ProductVariant productVariant, 
            int taxClassId, Customer customer)
        {
            var calculateTaxRequest = new CalculateTaxRequest();
            calculateTaxRequest.Customer = customer;
            calculateTaxRequest.Item = productVariant;
            calculateTaxRequest.TaxClassId = taxClassId;

            var basedOn = this.TaxBasedOn;

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
                        address = this.DefaultTaxAddress;
                    }
                    break;
                case TaxBasedOnEnum.ShippingOrigin:
                    {
                        address = IoC.Resolve<IShippingService>().ShippingOrigin;
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
        protected decimal CalculatePrice(decimal price, decimal percent, bool increase)
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
        public decimal GetTaxTotal(ShoppingCart cart,
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
        public decimal GetTaxTotal(ShoppingCart cart, int paymentMethodId, 
            Customer customer, ref string error)
        {
            SortedDictionary<decimal, decimal> taxRates = null;
            return  GetTaxTotal(cart, paymentMethodId, 
                customer, out taxRates, ref error);
        }

        /// <summary>
        /// Gets tax
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="paymentMethodId">Payment method identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRates">Tax rates</param>
        /// <param name="error">Error</param>
        /// <returns>Tax total</returns>
        public decimal GetTaxTotal(ShoppingCart cart, int paymentMethodId,
            Customer customer, out SortedDictionary<decimal, decimal> taxRates, ref string error)
        {
            decimal taxTotal = decimal.Zero;
            taxRates = new SortedDictionary<decimal, decimal>();

            //order sub total (items + checkout attributes)
            decimal subTotalTaxTotal = decimal.Zero;
            decimal orderSubTotalDiscountAmount = decimal.Zero;
            Discount orderSubTotalAppliedDiscount = null;
            decimal subTotalWithoutDiscountBase = decimal.Zero;
            decimal subTotalWithDiscountBase = decimal.Zero;
            SortedDictionary<decimal, decimal> orderSubTotalTaxRates = null;
            string subTotalError = IoC.Resolve<IShoppingCartService>().GetShoppingCartSubTotal(cart,
                customer, false, out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase, out orderSubTotalTaxRates);
            foreach (KeyValuePair<decimal,decimal> kvp in orderSubTotalTaxRates)
            {
                decimal taxRate = kvp.Key;
                decimal taxValue = kvp.Value;
                subTotalTaxTotal += taxValue;

                if (taxRate > decimal.Zero && taxValue > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(taxRate))
                    {
                        taxRates.Add(taxRate, taxValue);
                    }
                    else
                    {
                        taxRates[taxRate] = taxRates[taxRate] + taxValue;
                    }
                }
            }

            //shipping
            decimal shippingTax = decimal.Zero;
            if (this.ShippingIsTaxable)
            {
                decimal taxRate = decimal.Zero;
                string error1 = string.Empty;
                string error2 = string.Empty;

                decimal? shippingExclTax = IoC.Resolve<IShippingService>().GetShoppingCartShippingTotal(cart, customer, false, out taxRate, ref error1);
                decimal? shippingInclTax = IoC.Resolve<IShippingService>().GetShoppingCartShippingTotal(cart, customer, true, out taxRate, ref error2);
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

                    //tax rates
                    if (taxRate > decimal.Zero && shippingTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(taxRate))
                        {
                            taxRates.Add(taxRate, shippingTax);
                        }
                        else
                        {
                            taxRates[taxRate] = taxRates[taxRate] + shippingTax;
                        }
                    }
                }
            }

            //payment method additional fee
            decimal paymentMethodAdditionalFeeTax = decimal.Zero;
            if (this.PaymentMethodAdditionalFeeIsTaxable)
            {
                decimal taxRate = decimal.Zero;
                string error1 = string.Empty;
                string error2 = string.Empty;

                decimal paymentMethodAdditionalFee = IoC.Resolve<IPaymentService>().GetAdditionalHandlingFee(paymentMethodId);
                decimal? paymentMethodAdditionalFeeExclTax = this.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, false, customer, out taxRate, ref error1);
                decimal? paymentMethodAdditionalFeeInclTax = this.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, true, customer, out taxRate, ref error2);
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

                    //tax rates
                    if (taxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(taxRate))
                        {
                            taxRates.Add(taxRate, paymentMethodAdditionalFeeTax);
                        }
                        else
                        {
                            taxRates[taxRate] = taxRates[taxRate] + paymentMethodAdditionalFeeTax;
                        }
                    }
                }
            }

            //add at least one tax rate (0%)
            if (taxRates.Count == 0)
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            taxTotal = subTotalTaxTotal + shippingTax + paymentMethodAdditionalFeeTax;
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
        public decimal GetTaxRate(ProductVariant productVariant, 
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
        public decimal GetTaxRate(int taxClassId, Customer customer, ref string error)
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
        public decimal GetTaxRate(ProductVariant productVariant, 
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
            if (this.EUVatEnabled)
            {
                if (IsVatExempt(calculateTaxRequest.Address, calculateTaxRequest.Customer))
                {
                    //return zero if VAT is not chargeable
                    return decimal.Zero;
                }
            }

            //instantiate tax provider
            var activeTaxProvider = this.ActiveTaxProvider;
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
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetPrice(ProductVariant productVariant, decimal price, 
            out decimal taxRate)
        {
            string error = string.Empty;
            return GetPrice(productVariant, price, out taxRate, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public decimal GetPrice(ProductVariant productVariant, decimal price, 
            out decimal taxRate, ref string error)
        {
            var customer = NopContext.Current.User;
            return GetPrice(productVariant, price, customer, out taxRate, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetPrice(ProductVariant productVariant, decimal price,
            Customer customer, out decimal taxRate)
        {
            string error = string.Empty;
            return GetPrice(productVariant, price, customer, out taxRate, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public decimal GetPrice(ProductVariant productVariant, decimal price,
            Customer customer, out decimal taxRate, ref string error)
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
            return GetPrice(productVariant, price, includingTax, customer, out taxRate, ref error);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetPrice(ProductVariant productVariant, decimal price,
            bool includingTax, Customer customer, out decimal taxRate)
        {
            string error = string.Empty;
            return GetPrice(productVariant, price, includingTax, customer, out taxRate, ref error);
        }

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
        public decimal GetPrice(ProductVariant productVariant, decimal price,
            bool includingTax, Customer customer, out decimal taxRate, ref string error)
        {
            bool priceIncludesTax = this.PricesIncludeTax;
            int taxClassId = 0;
            return GetPrice(productVariant, taxClassId, price, includingTax, 
                customer, priceIncludesTax, out taxRate, ref error);
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
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Price</returns>
        public decimal GetPrice(ProductVariant productVariant, int taxClassId,
            decimal price, bool includingTax, Customer customer, out decimal taxRate, 
            bool priceIncludesTax)
        {
            string error = string.Empty;
            return GetPrice(productVariant, taxClassId, price, includingTax,
                customer, priceIncludesTax, out taxRate, ref error);           
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
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public decimal GetPrice(ProductVariant productVariant, int taxClassId,
            decimal price, bool includingTax, Customer customer,
            bool priceIncludesTax, out decimal taxRate, ref string error)
        {
            taxRate = GetTaxRate(productVariant, taxClassId, customer, ref error);
            
            if (priceIncludesTax)
            {
                if (!includingTax)
                {
                    price = CalculatePrice(price, taxRate, false);
                }
            }
            else
            {
                if (includingTax)
                {
                    price = CalculatePrice(price, taxRate, true);
                }
            }

            //allowed to support negative price adjustments
            //if (price < decimal.Zero)
            //    price = decimal.Zero;
            price = Math.Round(price, 2);

            return price;
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public decimal GetShippingPrice(decimal price, Customer customer)
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
        public decimal GetShippingPrice(decimal price,  Customer customer, 
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
        public decimal GetShippingPrice(decimal price, bool includingTax, Customer customer)
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
        public decimal GetShippingPrice(decimal price, bool includingTax, 
            Customer customer, ref string error)
        {
            decimal taxRate = decimal.Zero;
            return GetShippingPrice(price, includingTax, customer, out taxRate, ref error);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public decimal GetShippingPrice(decimal price, bool includingTax,
            Customer customer, out decimal taxRate, ref string error)
        {
            taxRate = decimal.Zero;

            if (!this.ShippingIsTaxable)
            {
                return price;
            }
            int taxClassId = this.ShippingTaxClassId;
            bool priceIncludesTax = this.ShippingPriceIncludesTax;
            return GetPrice(null, taxClassId, price, includingTax, customer, 
                priceIncludesTax, out taxRate, ref error);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        public decimal GetPaymentMethodAdditionalFee(decimal price, Customer customer)
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
        public decimal GetPaymentMethodAdditionalFee(decimal price, 
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
        public decimal GetPaymentMethodAdditionalFee(decimal price, bool includingTax, Customer customer)
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
        public decimal GetPaymentMethodAdditionalFee(decimal price, 
            bool includingTax, Customer customer, ref string error)
        {
            decimal taxRate = decimal.Zero;
            return GetPaymentMethodAdditionalFee(price, includingTax, 
                customer, out taxRate, ref error);
        }
        
        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public decimal GetPaymentMethodAdditionalFee(decimal price,
            bool includingTax, Customer customer, out decimal taxRate, ref string error)
        {
            taxRate = decimal.Zero;

            if (!this.PaymentMethodAdditionalFeeIsTaxable)
            {
                return price;
            }
            int taxClassId = this.PaymentMethodAdditionalFeeTaxClassId;
            bool priceIncludesTax = this.PaymentMethodAdditionalFeeIncludesTax;
            return GetPrice(null, taxClassId, price, includingTax, customer, 
                priceIncludesTax, out taxRate, ref error);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>Price</returns>
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav)
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
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, 
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
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, 
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
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav, 
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
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, Customer customer, ref string error)
        {
            decimal taxRate = decimal.Zero;
            return GetCheckoutAttributePrice(cav, includingTax, customer,
                out taxRate, ref error);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="taxRate">Tax rate</param>
        /// <param name="error">Error</param>
        /// <returns>Price</returns>
        public decimal GetCheckoutAttributePrice(CheckoutAttributeValue cav,
            bool includingTax, Customer customer, out decimal taxRate, ref string error)
        {
            if (cav == null)
                throw new ArgumentNullException("cav");

            taxRate = decimal.Zero;

            decimal price = cav.PriceAdjustment;
            if (cav.CheckoutAttribute.IsTaxExempt)
            {
                return price;
            }

            bool priceIncludesTax = this.PricesIncludeTax;
            int taxClassId = cav.CheckoutAttribute.TaxCategoryId;
            return GetPrice(null, taxClassId, price, includingTax, customer, 
                priceIncludesTax, out taxRate, ref error);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>VAT Number status</returns>
        public VatNumberStatusEnum GetVatNumberStatus(Country country,
            string vatNumber)
        {
            string name = string.Empty;
            string address = string.Empty;
            return GetVatNumberStatus(country, vatNumber, out name, out address);
        }
        
        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <param name="name">Name (if received)</param>
        /// <param name="address">Address (if received)</param>
        /// <returns>VAT Number status</returns>
        public VatNumberStatusEnum GetVatNumberStatus(Country country,
            string vatNumber, out string name, out string address)
        {
            name = string.Empty;
            address = string.Empty;

            if (vatNumber == null)
                vatNumber = string.Empty;
            vatNumber = vatNumber.Trim();

            if (String.IsNullOrEmpty(vatNumber))
                return VatNumberStatusEnum.Empty;


            if (country == null)
                return VatNumberStatusEnum.Unknown;

            if (!this.EUVatUseWebService)
                return VatNumberStatusEnum.Unknown;


            //UNDONE
            try
            {
                Exception exception = null;
                return DoVatCheck(country.TwoLetterIsoCode, vatNumber, out name, out address, out exception);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
                return VatNumberStatusEnum.Unknown;
            }
        }

        /// <summary>
        /// Gets VAT Number status name
        /// </summary>
        /// <param name="status">VAT Number status</param>
        /// <returns>VAT Number status name</returns>
        public string GetVatNumberStatusName(VatNumberStatusEnum status)
        {
            return IoC.Resolve<ILocalizationManager>().GetLocaleResourceString(string.Format("VatNumberStatus.{0}", status.ToString()));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Tax based on
        /// </summary>
        public TaxBasedOnEnum TaxBasedOn
        {
            get
            {
                int taxBasedOn = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Tax.TaxBasedOn");
                return (TaxBasedOnEnum)taxBasedOn;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.TaxBasedOn", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Tax display type
        /// </summary>
        public TaxDisplayTypeEnum TaxDisplayType
        {
            get
            {
                int taxBasedOn = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Tax.TaxDisplayType");
                return (TaxDisplayTypeEnum)taxBasedOn;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.TaxDisplayType", ((int)value).ToString());
            }
        }
        
        /// <summary>
        /// Gets or sets an active shipping rate computation method
        /// </summary>
        public TaxProvider ActiveTaxProvider
        {
            get
            {
                int taxProviderId = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Tax.TaxProvider.ActiveId");
                return IoC.Resolve<ITaxProviderService>().GetTaxProviderById(taxProviderId);
            }
            set
            {
                if (value != null)
                    IoC.Resolve<ISettingManager>().SetParam("Tax.TaxProvider.ActiveId", value.TaxProviderId.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a default tax address
        /// </summary>
        public Address DefaultTaxAddress
        {
            get
            {
                int countryId = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Tax.DefaultTaxAddress.CountryId");
                int stateProvinceId = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Tax.DefaultTaxAddress.StateProvinceId");
                string zipPostalCode = IoC.Resolve<ISettingManager>().GetSettingValue("Tax.DefaultTaxAddress.ZipPostalCode");
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

                IoC.Resolve<ISettingManager>().SetParam("Tax.DefaultTaxAddress.CountryId", countryId.ToString());
                IoC.Resolve<ISettingManager>().SetParam("Tax.DefaultTaxAddress.StateProvinceId", stateProvinceId.ToString());
                IoC.Resolve<ISettingManager>().SetParam("Tax.DefaultTaxAddress.ZipPostalCode", zipPostalCode);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display tax suffix
        /// </summary>
        public bool DisplayTaxSuffix
        {
            get
            {
                bool displayTaxSuffix = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.DisplayTaxSuffix");
                return displayTaxSuffix;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.DisplayTaxSuffix", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether each tax rate should be displayed on separate line (shopping cart page)
        /// </summary>
        public bool DisplayTaxRates
        {
            get
            {
                bool displayTaxRate = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.DisplayTaxRates", false);
                return displayTaxRate;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.DisplayTaxRates", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether prices incude tax
        /// </summary>
        public bool PricesIncludeTax
        {
            get
            {
                bool pricesIncludeTax = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.PricesIncludeTax");
                return pricesIncludeTax;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.PricesIncludeTax", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether customers are allowed to select tax display type
        /// </summary>
        public bool AllowCustomersToSelectTaxDisplayType
        {
            get
            {
                bool allowCustomersToSelectTaxDisplayType = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.AllowCustomersToSelectTaxDisplayType");
                return allowCustomersToSelectTaxDisplayType;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.AllowCustomersToSelectTaxDisplayType", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide zero tax
        /// </summary>
        public bool HideZeroTax
        {
            get
            {
                bool hideZeroTax = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.HideZeroTax");
                return hideZeroTax;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.HideZeroTax", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide tax in order summary when prices are shown tax inclusive
        /// </summary>
        public bool HideTaxInOrderSummary
        {
            get
            {
                bool hideTaxInOrderSummary = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.HideTaxInOrderSummary");
                return hideTaxInOrderSummary;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.HideTaxInOrderSummary", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether shipping price is taxable
        /// </summary>
        public bool ShippingIsTaxable
        {
            get
            {
                bool shippingIsTaxable = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.ShippingIsTaxable");
                return shippingIsTaxable;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.ShippingIsTaxable", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether shipping price incudes tax
        /// </summary>
        public bool ShippingPriceIncludesTax
        {
            get
            {
                bool shippingPriceIncludesTax = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.ShippingPriceIncludesTax");
                return shippingPriceIncludesTax;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.ShippingPriceIncludesTax", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the shipping tax class identifier
        /// </summary>
        public int ShippingTaxClassId
        {
            get
            {
                int shippingTaxClassId = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Tax.ShippingTaxClassId");
                return shippingTaxClassId;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.ShippingTaxClassId", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether payment method additional fee is taxable
        /// </summary>
        public bool PaymentMethodAdditionalFeeIsTaxable
        {
            get
            {
                bool paymentMethodAdditionalFeeIsTaxable = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.PaymentMethodAdditionalFeeIsTaxable");
                return paymentMethodAdditionalFeeIsTaxable;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.PaymentMethodAdditionalFeeIsTaxable", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether payment method additional fee incudes tax
        /// </summary>
        public bool PaymentMethodAdditionalFeeIncludesTax
        {
            get
            {
                bool paymentMethodAdditionalFeeIncludesTax = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.PaymentMethodAdditionalFeeIncludesTax");
                return paymentMethodAdditionalFeeIncludesTax;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.PaymentMethodAdditionalFeeIncludesTax", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the payment method additional fee tax class identifier
        /// </summary>
        public int PaymentMethodAdditionalFeeTaxClassId
        {
            get
            {
                int paymentMethodAdditionalFeeTaxClassId = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Tax.PaymentMethodAdditionalFeeTaxClassId");
                return paymentMethodAdditionalFeeTaxClassId;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.PaymentMethodAdditionalFeeTaxClassId", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether EU VAT (Eupore Union Value Added Tax) is enabled
        /// </summary>
        public bool EUVatEnabled
        {
            get
            {
                bool result = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.EUVat.Enabled");
                return result;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.EUVat.Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a shop country identifier
        /// </summary>
        public int EUVatShopCountryId
        {
            get
            {
                int result = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Tax.EUVat.EUVatShopCountryId");
                return result;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.EUVat.EUVatShopCountryId", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this store will exempt eligible VAT-registered customers from VAT
        /// </summary>
        public bool EUVatAllowVATExemption
        {
            get
            {
                bool result = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.EUVat.AllowVATExemption", true);
                return result;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.EUVat.AllowVATExemption", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we should use the EU web service to validate VAT numbers
        /// </summary>
        public bool EUVatUseWebService
        {
            get
            {
                bool result = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.EUVat.UseWebService");
                return result;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.EUVat.UseWebService", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we should notify a store owner when a new VAT number is submitted
        /// </summary>
        public bool EUVatEmailAdminWhenNewVATSubmitted
        {
            get
            {
                bool result = IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Tax.EUVat.EUVatEmailAdminWhenNewVATSubmitted");
                return result;
            }
            set
            {
                IoC.Resolve<ISettingManager>().SetParam("Tax.EUVat.EUVatEmailAdminWhenNewVATSubmitted", value.ToString());
            }
        }

        #endregion
    }
}
