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
using System.Text;
using System.Web.Compilation;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Tax;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Price helper
    /// </summary>
    public partial class PriceHelper
    {
        #region Utilities

        /// <summary>
        /// Gets allowed discounts
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected static List<Discount> GetAllowedDiscounts(ProductVariant productVariant, Customer customer)
        {
            var allowedDiscounts = new List<Discount>();

            string customerCouponCode = string.Empty;
            if (customer != null)
                customerCouponCode = customer.LastAppliedCouponCode;

            foreach (var _discount in productVariant.AllDiscounts)
            {
                if (_discount.IsActive(customerCouponCode) &&
                    _discount.DiscountType == DiscountTypeEnum.AssignedToSKUs &&
                    !allowedDiscounts.ContainsDiscount(_discount.Name))
                {
                    //discount requirements
                    if (_discount.CheckDiscountRequirements(customer)
                        && _discount.CheckDiscountLimitations(customer))
                    {
                        allowedDiscounts.Add(_discount);
                    }
                }
            }

            var productCategories = IoCFactory.Resolve<ICategoryManager>().GetProductCategoriesByProductId(productVariant.ProductId);
            foreach (var _productCategory in productCategories)
            {
                //UNDONE should we filter categories by ACL here?
                var _categoryDiscounts = IoCFactory.Resolve<IDiscountManager>().GetDiscountsByCategoryId(_productCategory.CategoryId);
                foreach (var _discount in _categoryDiscounts)
                {
                    if (_discount.IsActive(customerCouponCode) &&
                        _discount.DiscountType == DiscountTypeEnum.AssignedToCategories &&
                        !allowedDiscounts.ContainsDiscount(_discount.Name))
                    {
                        //discount requirements
                        if (_discount.CheckDiscountRequirements(customer)
                            && _discount.CheckDiscountLimitations(customer))
                        {
                            allowedDiscounts.Add(_discount);
                        }
                    }
                }
            }
            return allowedDiscounts;
        }

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>Preferred discount</returns>
        protected static Discount GetPreferredDiscount(ProductVariant productVariant, 
            Customer customer)
        {
            return GetPreferredDiscount(productVariant, customer, decimal.Zero);
        }

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <returns>Preferred discount</returns>
        protected static Discount GetPreferredDiscount(ProductVariant productVariant, 
            Customer customer, decimal additionalCharge)
        {
            return GetPreferredDiscount(productVariant, customer, additionalCharge, 1);
        }

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="quantity">Product quantity</param>
        /// <returns>Preferred discount</returns>
        protected static Discount GetPreferredDiscount(ProductVariant productVariant,
            Customer customer, decimal additionalCharge, int quantity)
        {
            var allowedDiscounts = GetAllowedDiscounts(productVariant, customer);
            decimal finalPriceWithoutDiscount = GetFinalPrice(productVariant, customer, additionalCharge, false, quantity);
            var preferredDiscount = IoCFactory.Resolve<IDiscountManager>().GetPreferredDiscount(allowedDiscounts, finalPriceWithoutDiscount);
            return preferredDiscount;
        }
      
        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Price</returns>
        protected static decimal GetTierPrice(ProductVariant productVariant, int quantity)
        {
            var tierPrices = productVariant.TierPrices;

            int previousQty = 1;
            decimal previousPrice = productVariant.Price;            
            foreach (TierPrice tierPrice in tierPrices)
            {
                if (quantity < tierPrice.Quantity)
                    continue;

                if (tierPrice.Quantity < previousQty)
                    continue;

                previousPrice = tierPrice.Price; 
                previousQty = tierPrice.Quantity;
            }

            return  previousPrice;
        }

        /// <summary>
        /// Gets a price by customer role (if defined)
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        protected static decimal? GetCustomPriceByCustomerRole(ProductVariant productVariant, Customer customer)
        {
            if (productVariant == null)
                return null;
            if (customer == null)
                return null;

            decimal? result = null;
            var customerRoles = customer.CustomerRoles;
            var crppCollection = productVariant.CustomerRoleProductPrices;
            foreach (var crpp in crppCollection)
            {
                foreach (var cr in customerRoles)
                {
                    if (cr.CustomerRoleId == crpp.CustomerRoleId)
                    {
                        if (result.HasValue)
                        {
                            if (result.Value > crpp.Price)
                            {
                                result = crpp.Price;
                            }
                        }
                        else
                        {
                            result = crpp.Price;
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Methods

        #region Calculation methods

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        public static decimal GetFinalPrice(ProductVariant productVariant, 
            bool includeDiscounts)
        {
            var customer = NopContext.Current.User;
            return GetFinalPrice(productVariant, customer, includeDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        public static decimal GetFinalPrice(ProductVariant productVariant, Customer customer, 
            bool includeDiscounts)
        {
            return GetFinalPrice(productVariant, customer, decimal.Zero, includeDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        public static decimal GetFinalPrice(ProductVariant productVariant, Customer customer, 
            decimal additionalCharge, bool includeDiscounts)
        {
            return GetFinalPrice(productVariant, customer, additionalCharge, 
                includeDiscounts, 1);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price</returns>
        public static decimal GetFinalPrice(ProductVariant productVariant, Customer customer,
            decimal additionalCharge, bool includeDiscounts, int quantity)
        {
            decimal result = decimal.Zero;

            //initial price
            decimal initialPrice = productVariant.Price;

            //price by customer role
            decimal? cpcc = GetCustomPriceByCustomerRole(productVariant, customer);
            if (cpcc.HasValue)
            {
                initialPrice = cpcc.Value;
            }
            
            //tier prices
            if (productVariant.TierPrices.Count > 0)
            {
                decimal tierPrice = GetTierPrice(productVariant, quantity);
                initialPrice = Math.Min(initialPrice, tierPrice);
            }

            //discount + additional charge
            if (includeDiscounts)
            {
                Discount appliedDiscount = null;
                decimal discountAmount = GetDiscountAmount(productVariant, customer, additionalCharge, quantity, out appliedDiscount);
                result = initialPrice + additionalCharge - discountAmount;
            }
            else
            {
                result = initialPrice + additionalCharge;
            }
            if (result < decimal.Zero)
                result = decimal.Zero;
            return result;
        }

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
        public static decimal GetSubTotal(ShoppingCartItem shoppingCartItem, bool includeDiscounts)
        {
            var customer = NopContext.Current.User;
            return GetSubTotal(shoppingCartItem, customer, includeDiscounts);
        }

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="customer">The customer</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
        public static decimal GetSubTotal(ShoppingCartItem shoppingCartItem, Customer customer, 
            bool includeDiscounts)
        {
            return GetUnitPrice(shoppingCartItem, customer, includeDiscounts) * shoppingCartItem.Quantity;
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public static decimal GetUnitPrice(ShoppingCartItem shoppingCartItem, bool includeDiscounts)
        {
            var customer = NopContext.Current.User;
            return GetUnitPrice(shoppingCartItem, customer, includeDiscounts);
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="customer">The customer</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public static decimal GetUnitPrice(ShoppingCartItem shoppingCartItem, Customer customer,
            bool includeDiscounts)
        {
            decimal finalPrice = decimal.Zero;
            var productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
            {
                decimal attributesTotalPrice = decimal.Zero;

                var pvaValues = ProductAttributeHelper.ParseProductVariantAttributeValues(shoppingCartItem.AttributesXml);
                foreach (var pvaValue in pvaValues)
                {
                    attributesTotalPrice += pvaValue.PriceAdjustment;
                }

                if (productVariant.CustomerEntersPrice)
                {
                    finalPrice = shoppingCartItem.CustomerEnteredPrice;
                }
                else
                {
                    finalPrice = GetFinalPrice(productVariant, 
                        customer, 
                        attributesTotalPrice, 
                        includeDiscounts, 
                        shoppingCartItem.Quantity);
                }
            }

            finalPrice = Math.Round(finalPrice, 2);

            return finalPrice;
        }



        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant)
        {
            var customer = NopContext.Current.User;
            return GetDiscountAmount(productVariant, customer, decimal.Zero);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant, Customer customer)
        {
            return GetDiscountAmount(productVariant, customer, decimal.Zero);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant, Customer customer, 
            decimal additionalCharge)
        {
            Discount appliedDiscount = null;
            return GetDiscountAmount(productVariant, customer, additionalCharge, out appliedDiscount);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant, Customer customer,
            decimal additionalCharge, out Discount appliedDiscount)
        {
            return GetDiscountAmount(productVariant, customer, additionalCharge, 1, out appliedDiscount);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="quantity">Product quantity</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant, Customer customer,
            decimal additionalCharge, int quantity, out Discount appliedDiscount)
        {
            decimal appliedDiscountAmount = decimal.Zero;

            //we don't apply discounts to products with price entered by a customer
            if (productVariant.CustomerEntersPrice)
            {
                appliedDiscount = null;
                return appliedDiscountAmount;
            }

            appliedDiscount = GetPreferredDiscount(productVariant, customer, additionalCharge, quantity);
            if (appliedDiscount != null)
            {
                decimal finalPriceWithoutDiscount = GetFinalPrice(productVariant, customer, additionalCharge, false, quantity);
                appliedDiscountAmount = appliedDiscount.GetDiscountAmount(finalPriceWithoutDiscount);
            }

            return appliedDiscountAmount;
        }



        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ShoppingCartItem shoppingCartItem)
        {
            var customer = NopContext.Current.User;
            return GetDiscountAmount(shoppingCartItem, customer);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="customer">The customer</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ShoppingCartItem shoppingCartItem, Customer customer)
        {
            Discount appliedDiscount = null;
            return GetDiscountAmount(shoppingCartItem, customer, out appliedDiscount);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="customer">The customer</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ShoppingCartItem shoppingCartItem, Customer customer,
            out Discount appliedDiscount)
        {
            appliedDiscount = null;
            decimal discountAmount = decimal.Zero;
            var productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
            {
                decimal attributesTotalPrice = decimal.Zero;

                var pvaValues = ProductAttributeHelper.ParseProductVariantAttributeValues(shoppingCartItem.AttributesXml);
                foreach (var pvaValue in pvaValues)
                {
                    attributesTotalPrice += pvaValue.PriceAdjustment;
                }

                decimal productVariantDiscountAmount = GetDiscountAmount(productVariant, customer, attributesTotalPrice, shoppingCartItem.Quantity, out appliedDiscount);
                discountAmount = productVariantDiscountAmount * shoppingCartItem.Quantity;
            }

            discountAmount = Math.Round(discountAmount, 2);
            return discountAmount;
        }
        
        #endregion
        
        #region Formatting

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price)
        {
            bool ShowCurrency = true;
            var TargetCurrency = NopContext.Current.WorkingCurrency;
            return FormatPrice(price, ShowCurrency, TargetCurrency);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency)
        {
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, bool showTax)
        {
            var targetCurrency = NopContext.Current.WorkingCurrency;
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, 
            string currencyCode, bool showTax)
        {
            var currency = IoCFactory.Resolve<ICurrencyManager>().GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }

            return FormatPrice(price, showCurrency, currency, 
                language, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency,
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = IoCFactory.Resolve<ICurrencyManager>().GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            return FormatPrice(price, showCurrency, currency, language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = IoCFactory.Resolve<ITaxManager>().DisplayTaxSuffix;
            return FormatPrice(price, showCurrency, targetCurrency, language, 
                priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            string currencyString = LocalizationManager.GetCurrencyString(price, showCurrency, targetCurrency);

            if (showTax)
            {
                string formatStr = string.Empty;
                if (priceIncludesTax)
                {
                    formatStr = LocalizationManager.GetLocaleResourceString("Products.InclTaxSuffix", language.LanguageId, false);
                    if (String.IsNullOrEmpty(formatStr))
                    {
                        formatStr = "{0} incl tax";
                    }
                }
                else
                {
                    formatStr = LocalizationManager.GetLocaleResourceString("Products.ExclTaxSuffix", language.LanguageId, false);
                    if (String.IsNullOrEmpty(formatStr))
                    {
                        formatStr = "{0} excl tax";
                    }
                }
                string taxString = string.Format(formatStr, currencyString);
                return taxString;
            }
            else
            {
                return currencyString;
            }
        }



        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>Price</returns>
        public static string FormatShippingPrice(decimal price, bool showCurrency)
        {
            var targetCurrency = NopContext.Current.WorkingCurrency;
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatShippingPrice(price, showCurrency, targetCurrency, language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatShippingPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = IoCFactory.Resolve<ITaxManager>().ShippingIsTaxable && IoCFactory.Resolve<ITaxManager>().DisplayTaxSuffix;
            return FormatShippingPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatShippingPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }
        
        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatShippingPrice(decimal price, bool showCurrency, 
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = IoCFactory.Resolve<ICurrencyManager>().GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            return FormatShippingPrice(price, showCurrency, currency, language, priceIncludesTax);
        }



        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>Price</returns>
        public static string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency)
        {
            var targetCurrency = NopContext.Current.WorkingCurrency;
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPaymentMethodAdditionalFee(price, showCurrency, targetCurrency, 
                language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency,
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = IoCFactory.Resolve<ITaxManager>().PaymentMethodAdditionalFeeIsTaxable && IoCFactory.Resolve<ITaxManager>().DisplayTaxSuffix;
            return FormatPaymentMethodAdditionalFee(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language, 
                priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, 
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = IoCFactory.Resolve<ICurrencyManager>().GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            return FormatPaymentMethodAdditionalFee(price, showCurrency, currency, 
                language, priceIncludesTax);
        }



        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <returns>The stock message</returns>
        public static string FormatStockMessage(ProductVariant productVariant)
        {
            if (productVariant == null)
                throw new ArgumentNullException("productVariant");

            string stockMessage = string.Empty;

            if (productVariant.ManageInventory == (int)ManageInventoryMethodEnum.ManageStock
                && productVariant.DisplayStockAvailability)
            {
                switch (productVariant.Backorders)
                {
                    case (int)BackordersModeEnum.NoBackorders:
                        {
                            if (productVariant.StockQuantity > 0)
                            {
                                if (productVariant.DisplayStockQuantity)
                                {
                                    //display "in stock" with stock quantity
                                    stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), string.Format(LocalizationManager.GetLocaleResourceString("Products.InStockWithQuantity"), productVariant.StockQuantity));
                                }
                                else
                                {
                                    //display "in stock" without stock quantity
                                    stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), LocalizationManager.GetLocaleResourceString("Products.InStock"));
                                }
                            }
                            else
                            {
                                //display "out of stock"
                                stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), LocalizationManager.GetLocaleResourceString("Products.OutOfStock"));
                            }
                        }
                        break;
                    case (int)BackordersModeEnum.AllowQtyBelow0:
                        {
                            if (productVariant.StockQuantity > 0)
                            {
                                if (productVariant.DisplayStockQuantity)
                                {
                                    //display "in stock" with stock quantity
                                    stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), string.Format(LocalizationManager.GetLocaleResourceString("Products.InStockWithQuantity"), productVariant.StockQuantity));
                                }
                                else
                                {
                                    //display "in stock" without stock quantity
                                    stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), LocalizationManager.GetLocaleResourceString("Products.InStock"));
                                }
                            }
                            else
                            {
                                //display "in stock" without stock quantity
                                stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), LocalizationManager.GetLocaleResourceString("Products.InStock"));
                            }
                        }
                        break;
                    case (int)BackordersModeEnum.AllowQtyBelow0AndNotifyCustomer:
                        {
                            if (productVariant.StockQuantity > 0)
                            {
                                if (productVariant.DisplayStockQuantity)
                                {
                                    //display "in stock" with stock quantity
                                    stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), string.Format(LocalizationManager.GetLocaleResourceString("Products.InStockWithQuantity"), productVariant.StockQuantity));
                                }
                                else
                                {
                                    //display "in stock" without stock quantity
                                    stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), LocalizationManager.GetLocaleResourceString("Products.InStock"));
                                }
                            }
                            else
                            {
                                //display "backorder" without stock quantity
                                stockMessage = string.Format(LocalizationManager.GetLocaleResourceString("Products.Availability"), LocalizationManager.GetLocaleResourceString("Products.Backordering"));
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return stockMessage;
        }

        #endregion

        #endregion
    }
}
