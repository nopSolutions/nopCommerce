using System;
using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Price formatter
    /// </summary>
    public partial class PriceFormatter : IPriceFormatter
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IWorkContext _workContext;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public PriceFormatter(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IWorkContext workContext,
            TaxSettings taxSettings)
        {
            this._currencySettings = currencySettings;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._measureService = measureService;
            this._workContext = workContext;
            this._taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Currency string without exchange rate</returns>
        protected virtual string GetCurrencyString(decimal amount)
        {
            return GetCurrencyString(amount, true, _workContext.WorkingCurrency);
        }

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Currency string without exchange rate</returns>
        protected virtual string GetCurrencyString(decimal amount,
            bool showCurrency, Currency targetCurrency)
        {
            if (targetCurrency == null)
                throw new ArgumentNullException(nameof(targetCurrency));

            string result;
            if (!string.IsNullOrEmpty(targetCurrency.CustomFormatting))
            {
                //custom formatting specified by a store owner
                result = amount.ToString(targetCurrency.CustomFormatting);
            }
            else
            {
                if (!string.IsNullOrEmpty(targetCurrency.DisplayLocale))
                {
                    //default behavior
                    result = amount.ToString("C", new CultureInfo(targetCurrency.DisplayLocale));
                }
                else
                {
                    //not possible because "DisplayLocale" should be always specified
                    //but anyway let's just handle this behavior
                    result = $"{amount:N} ({targetCurrency.CurrencyCode})";
                    return result;
                }
            }

            //display currency code?
            if (showCurrency && _currencySettings.DisplayCurrencyLabel)
                result = $"{result} ({targetCurrency.CurrencyCode})";
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <returns>Price</returns>
        public virtual string FormatPrice(decimal price)
        {
            return FormatPrice(price, true, _workContext.WorkingCurrency);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Price</returns>
        public virtual string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency)
        {
            var priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatPrice(price, showCurrency, targetCurrency, _workContext.WorkingLanguage, priceIncludesTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public virtual string FormatPrice(decimal price, bool showCurrency, bool showTax)
        {
            var priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatPrice(price, showCurrency, _workContext.WorkingCurrency, _workContext.WorkingLanguage, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <param name="language">Language</param>
        /// <returns>Price</returns>
        public virtual string FormatPrice(decimal price, bool showCurrency,
            string currencyCode, bool showTax, Language language)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode) ?? new Currency
            {
                CurrencyCode = currencyCode
            };

            var priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatPrice(price, showCurrency, currency, language, priceIncludesTax, showTax);
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
        public virtual string FormatPrice(decimal price, bool showCurrency,
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode)
                ?? new Currency
                {
                    CurrencyCode = currencyCode
                };
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
        public virtual string FormatPrice(decimal price, bool showCurrency,
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language,
                priceIncludesTax, _taxSettings.DisplayTaxSuffix);
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
        public virtual string FormatPrice(decimal price, bool showCurrency,
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            //we should round it no matter of "ShoppingCartSettings.RoundPricesDuringCalculation" setting
            var priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
            price = priceCalculationService.RoundPrice(price, targetCurrency);

            var currencyString = GetCurrencyString(price, showCurrency, targetCurrency);
            if (!showTax) 
                return currencyString;

            //show tax suffix
            string formatStr;
            if (priceIncludesTax)
            {
                formatStr = _localizationService.GetResource("Products.InclTaxSuffix", language.Id, false);
                if (string.IsNullOrEmpty(formatStr))
                    formatStr = "{0} incl tax";
            }
            else
            {
                formatStr = _localizationService.GetResource("Products.ExclTaxSuffix", language.Id, false);
                if (string.IsNullOrEmpty(formatStr))
                    formatStr = "{0} excl tax";
            }

            return string.Format(formatStr, currencyString);
        }

        /// <summary>
        /// Formats the price of rental product (with rental period)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <returns>Rental product price with period</returns>
        public virtual string FormatRentalProductPeriod(Product product, string price)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (!product.IsRental)
                return price;

            if (string.IsNullOrWhiteSpace(price))
                return price;

            string result;
            switch (product.RentalPricePeriod)
            {
                case RentalPricePeriod.Days:
                    result = string.Format(_localizationService.GetResource("Products.Price.Rental.Days"), price, product.RentalPriceLength);
                    break;
                case RentalPricePeriod.Weeks:
                    result = string.Format(_localizationService.GetResource("Products.Price.Rental.Weeks"), price, product.RentalPriceLength);
                    break;
                case RentalPricePeriod.Months:
                    result = string.Format(_localizationService.GetResource("Products.Price.Rental.Months"), price, product.RentalPriceLength);
                    break;
                case RentalPricePeriod.Years:
                    result = string.Format(_localizationService.GetResource("Products.Price.Rental.Years"), price, product.RentalPriceLength);
                    break;
                default:
                    throw new NopException("Not supported rental period");
            }

            return result;
        }

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>Price</returns>
        public virtual string FormatShippingPrice(decimal price, bool showCurrency)
        {
            var priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatShippingPrice(price, showCurrency, _workContext.WorkingCurrency, _workContext.WorkingLanguage, priceIncludesTax);
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
        public virtual string FormatShippingPrice(decimal price, bool showCurrency,
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            var showTax = _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix;
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
        public virtual string FormatShippingPrice(decimal price, bool showCurrency,
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
        public virtual string FormatShippingPrice(decimal price, bool showCurrency,
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode)
                ?? new Currency
                {
                    CurrencyCode = currencyCode
                };
            return FormatShippingPrice(price, showCurrency, currency, language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>Price</returns>
        public virtual string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency)
        {
            var priceIncludesTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
            return FormatPaymentMethodAdditionalFee(price, showCurrency, _workContext.WorkingCurrency,
                _workContext.WorkingLanguage, priceIncludesTax);
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
        public virtual string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency,
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            var showTax = _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix;
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
        public virtual string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency,
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
        public virtual string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency,
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = _currencyService.GetCurrencyByCode(currencyCode)
                ?? new Currency
                {
                    CurrencyCode = currencyCode
                };
            return FormatPaymentMethodAdditionalFee(price, showCurrency, currency,
                language, priceIncludesTax);
        }

        /// <summary>
        /// Formats a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Formatted tax rate</returns>
        public virtual string FormatTaxRate(decimal taxRate)
        {
            return taxRate.ToString("G29");
        }

        /// <summary>
        /// Format base price (PAngV)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="productPrice">Product price (in primary currency). Pass null if you want to use a default produce price</param>
        /// <param name="totalWeight">Total weight of product (with attribute weight adjustment). Pass null if you want to use a default produce weight</param>
        /// <returns>Base price</returns>
        public virtual string FormatBasePrice(Product product, decimal? productPrice, decimal? totalWeight = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (!product.BasepriceEnabled)
                return null;

            var productAmount = totalWeight.HasValue && totalWeight.Value > decimal.Zero ? totalWeight.Value : product.BasepriceAmount;
            //Amount in product cannot be 0
            if (productAmount == 0)
                return null;
            var referenceAmount = product.BasepriceBaseAmount;
            var productUnit = _measureService.GetMeasureWeightById(product.BasepriceUnitId);
            //measure weight cannot be loaded
            if (productUnit == null)
                return null;
            var referenceUnit = _measureService.GetMeasureWeightById(product.BasepriceBaseUnitId);
            //measure weight cannot be loaded
            if (referenceUnit == null)
                return null;

            productPrice = productPrice ?? product.Price;

            var basePrice = productPrice.Value /
                //do not round. otherwise, it can cause issues
                _measureService.ConvertWeight(productAmount, productUnit, referenceUnit, false) *
                referenceAmount;
            var basePriceInCurrentCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(basePrice, _workContext.WorkingCurrency);
            var basePriceStr = this.FormatPrice(basePriceInCurrentCurrency, true, false);

            var result = string.Format(_localizationService.GetResource("Products.BasePrice"),
                basePriceStr, referenceAmount.ToString("G29"), referenceUnit.Name);
            return result;
        }

        #endregion
    }
}