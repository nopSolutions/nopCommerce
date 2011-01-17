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
using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Price formatter
    /// </summary>
    public partial class PriceFormatter : IPriceFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly TaxSettings _taxSettings;

        public PriceFormatter(IWorkContext workContext,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            TaxSettings taxSettings)
        {
            this._workContext = workContext;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._taxSettings = taxSettings;
        }

        #region Utilities

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Currency string without exchange rate</returns>
        protected string GetCurrencyString(decimal amount)
        {
            bool showCurrency = true;
            var targetCurrency = _workContext.WorkingCurrency;
            return GetCurrencyString(amount, showCurrency, targetCurrency);
        }

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Currency string without exchange rate</returns>
        protected string GetCurrencyString(decimal amount,
            bool showCurrency, Currency targetCurrency)
        {
            string result = string.Empty;
            if (!String.IsNullOrEmpty(targetCurrency.CustomFormatting))
            {
                result = amount.ToString(targetCurrency.CustomFormatting);
            }
            else
            {
                if (!String.IsNullOrEmpty(targetCurrency.DisplayLocale))
                {
                    result = amount.ToString("C", new CultureInfo(targetCurrency.DisplayLocale));
                }
                else
                {
                    result = String.Format("{0} ({1})", amount.ToString("N"), targetCurrency.CurrencyCode);
                    return result;
                }
            }

            if (showCurrency && _currencyService.GetAllCurrencies().Count > 1)
                result = String.Format("{0} ({1})", result, targetCurrency.CurrencyCode);
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <returns>Price</returns>
        public string FormatPrice(decimal price)
        {
            bool showCurrency = true;
            var targetCurrency = _workContext.WorkingCurrency;
            return FormatPrice(price, showCurrency, targetCurrency);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Price</returns>
        public string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency)
        {
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
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
        public string FormatPrice(decimal price, bool showCurrency, bool showTax)
        {
            var targetCurrency = _workContext.WorkingCurrency;
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
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
        public string FormatPrice(decimal price, bool showCurrency, 
            string currencyCode, bool showTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
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
        public string FormatPrice(decimal price, bool showCurrency,
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode);
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
        public string FormatPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = _taxSettings.DisplayTaxSuffix;
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
        public string FormatPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            string currencyString = GetCurrencyString(price, showCurrency, targetCurrency);

            if (showTax)
            {
                string formatStr = string.Empty;
                if (priceIncludesTax)
                {
                    formatStr = _localizationService.GetLocaleResourceString("Products.InclTaxSuffix", language.Id, false);
                    if (String.IsNullOrEmpty(formatStr))
                    {
                        formatStr = "{0} incl tax";
                    }
                }
                else
                {
                    formatStr = _localizationService.GetLocaleResourceString("Products.ExclTaxSuffix", language.Id, false);
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
        public string FormatShippingPrice(decimal price, bool showCurrency)
        {
            var targetCurrency = _workContext.WorkingCurrency;
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
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
        public string FormatShippingPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix;
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
        public string FormatShippingPrice(decimal price, bool showCurrency, 
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
        public string FormatShippingPrice(decimal price, bool showCurrency, 
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode);
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
        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency)
        {
            var targetCurrency = _workContext.WorkingCurrency;
            var language = _workContext.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (_workContext.TaxDisplayType)
            {
                case TaxDisplayType.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayType.IncludingTax:
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
        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency,
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix;
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
        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, 
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
        public string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, 
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            return FormatPaymentMethodAdditionalFee(price, showCurrency, currency, 
                language, priceIncludesTax);
        }



        /// <summary>
        /// Formats a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Formatted tax rate</returns>
        public string FormatTaxRate(decimal taxRate)
        {
            return taxRate.ToString("G29");
        }

        #endregion
    }
}
