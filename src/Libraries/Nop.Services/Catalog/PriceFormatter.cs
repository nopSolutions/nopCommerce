using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
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
        #region Fields

        protected readonly CurrencySettings _currencySettings;
        protected readonly ICurrencyService _currencyService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IMeasureService _measureService;
        protected readonly IPriceCalculationService _priceCalculationService;
        protected readonly IWorkContext _workContext;
        protected readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public PriceFormatter(CurrencySettings currencySettings,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPriceCalculationService priceCalculationService,
            IWorkContext workContext,
            TaxSettings taxSettings)
        {
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _measureService = measureService;
            _priceCalculationService = priceCalculationService;
            _workContext = workContext;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

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
                //custom formatting specified by a store owner
                result = amount.ToString(targetCurrency.CustomFormatting);
            else
            {
                if (!string.IsNullOrEmpty(targetCurrency.DisplayLocale))
                    //default behavior
                    result = amount.ToString("C", new CultureInfo(targetCurrency.DisplayLocale));
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

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        protected virtual async Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax, bool showTax)
        {
            return await FormatPriceAsync(price, showCurrency, targetCurrency, languageId, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        protected virtual async Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax, bool showTax)
        {
            return await FormatPriceAsync(price, showCurrency, targetCurrency, languageId,
                priceIncludesTax, showTax);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPriceAsync(decimal price)
        {
            return await FormatPriceAsync(price, true, await _workContext.GetWorkingCurrencyAsync());
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency, Currency targetCurrency)
        {
            var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            return await FormatPriceAsync(price, showCurrency, targetCurrency, (await _workContext.GetWorkingLanguageAsync()).Id, priceIncludesTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency, bool showTax)
        {
            var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            return await FormatPriceAsync(price, showCurrency, await _workContext.GetWorkingCurrencyAsync(), (await _workContext.GetWorkingLanguageAsync()).Id, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <param name="languageId">Language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency,
            string currencyCode, bool showTax, int languageId)
        {
            var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode) ?? new Currency
            {
                CurrencyCode = currencyCode
            };

            var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            return await FormatPriceAsync(price, showCurrency, currency, languageId, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the order price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="currencyRate">Currency rate</param>
        /// <param name="customerCurrencyCode">Customer currency code</param>
        /// <param name="displayCustomerCurrency">A value indicating whether to display price on customer currency</param>
        /// <param name="primaryStoreCurrency">Primary store currency</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatOrderPriceAsync(decimal price,
            decimal currencyRate, string customerCurrencyCode, bool displayCustomerCurrency,
            Currency primaryStoreCurrency, int languageId, bool? priceIncludesTax = null, bool? showTax = null)
        {
            var needAddPriceOnCustomerCurrency = primaryStoreCurrency.CurrencyCode != customerCurrencyCode && displayCustomerCurrency;
            var includesTax = priceIncludesTax ?? await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            var priceText = await FormatPriceAsync(price, true, primaryStoreCurrency,
                languageId, includesTax, showTax ?? _taxSettings.DisplayTaxSuffix);

            if (!needAddPriceOnCustomerCurrency || await _currencyService.GetCurrencyByCodeAsync(customerCurrencyCode) is not Currency currency)
                return priceText;

            var customerPrice = _currencyService.ConvertCurrency(price, currencyRate);
            var customerPriceText = await FormatPriceAsync(customerPrice, true, currency,
                languageId, includesTax, showTax ?? _taxSettings.DisplayTaxSuffix);
            priceText += $"<br />[{customerPriceText}]";

            return priceText;
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency,
            string currencyCode, int languageId, bool priceIncludesTax)
        {
            var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode)
                ?? new Currency
                {
                    CurrencyCode = currencyCode
                };
            return await FormatPriceAsync(price, showCurrency, currency, languageId, priceIncludesTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax)
        {
            return await FormatPriceAsync(price, showCurrency, targetCurrency, languageId,
                priceIncludesTax, _taxSettings.DisplayTaxSuffix);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPriceAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax, bool showTax)
        {
            //we should round it no matter of "ShoppingCartSettings.RoundPricesDuringCalculation" setting
            price = await _priceCalculationService.RoundPriceAsync(price, targetCurrency);

            var currencyString = GetCurrencyString(price, showCurrency, targetCurrency);
            if (!showTax)
                return currencyString;

            //show tax suffix
            string formatStr;
            if (priceIncludesTax)
            {
                formatStr = await _localizationService.GetResourceAsync("Products.InclTaxSuffix", languageId, false);
                if (string.IsNullOrEmpty(formatStr))
                    formatStr = "{0} incl tax";
            }
            else
            {
                formatStr = await _localizationService.GetResourceAsync("Products.ExclTaxSuffix", languageId, false);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rental product price with period
        /// </returns>
        public virtual async Task<string> FormatRentalProductPeriodAsync(Product product, string price)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (!product.IsRental)
                return price;

            if (string.IsNullOrWhiteSpace(price))
                return price;
            var result = product.RentalPricePeriod switch
            {
                RentalPricePeriod.Days => string.Format(await _localizationService.GetResourceAsync("Products.Price.Rental.Days"), price, product.RentalPriceLength),
                RentalPricePeriod.Weeks => string.Format(await _localizationService.GetResourceAsync("Products.Price.Rental.Weeks"), price, product.RentalPriceLength),
                RentalPricePeriod.Months => string.Format(await _localizationService.GetResourceAsync("Products.Price.Rental.Months"), price, product.RentalPriceLength),
                RentalPricePeriod.Years => string.Format(await _localizationService.GetResourceAsync("Products.Price.Rental.Years"), price, product.RentalPriceLength),
                _ => throw new NopException("Not supported rental period"),
            };
            return result;
        }

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency)
        {
            var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            return await FormatShippingPriceAsync(price, showCurrency, await _workContext.GetWorkingCurrencyAsync(), (await _workContext.GetWorkingLanguageAsync()).Id, priceIncludesTax);
        }

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax)
        {
            var showTax = _taxSettings.ShippingIsTaxable && _taxSettings.DisplayTaxSuffix;
            return await FormatShippingPriceAsync(price, showCurrency, targetCurrency, languageId, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency,
            string currencyCode, int languageId, bool priceIncludesTax)
        {
            var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode)
                ?? new Currency
                {
                    CurrencyCode = currencyCode
                };
            return await FormatShippingPriceAsync(price, showCurrency, currency, languageId, priceIncludesTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency)
        {
            var priceIncludesTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

            return await FormatPaymentMethodAdditionalFeeAsync(price, showCurrency, await _workContext.GetWorkingCurrencyAsync(),
                (await _workContext.GetWorkingLanguageAsync()).Id, priceIncludesTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax)
        {
            var showTax = _taxSettings.PaymentMethodAdditionalFeeIsTaxable && _taxSettings.DisplayTaxSuffix;
            return await FormatPaymentMethodAdditionalFeeAsync(price, showCurrency, targetCurrency, languageId, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="languageId">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        public virtual async Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency,
            string currencyCode, int languageId, bool priceIncludesTax)
        {
            var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode)
                ?? new Currency
                {
                    CurrencyCode = currencyCode
                };
            return await FormatPaymentMethodAdditionalFeeAsync(price, showCurrency, currency,
                languageId, priceIncludesTax);
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the base price
        /// </returns>
        public virtual async Task<string> FormatBasePriceAsync(Product product, decimal? productPrice, decimal? totalWeight = null)
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
            var productUnit = await _measureService.GetMeasureWeightByIdAsync(product.BasepriceUnitId);
            //measure weight cannot be loaded
            if (productUnit == null)
                return null;
            var referenceUnit = await _measureService.GetMeasureWeightByIdAsync(product.BasepriceBaseUnitId);
            //measure weight cannot be loaded
            if (referenceUnit == null)
                return null;

            productPrice ??= product.Price;

            var basePrice = productPrice.Value /
                //do not round. otherwise, it can cause issues
                await _measureService.ConvertWeightAsync(productAmount, productUnit, referenceUnit, false) *
                referenceAmount;
            var basePriceInCurrentCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(basePrice, await _workContext.GetWorkingCurrencyAsync());
            var basePriceStr = await FormatPriceAsync(basePriceInCurrentCurrency, true, false);

            var result = string.Format(await _localizationService.GetResourceAsync("Products.BasePrice"),
                basePriceStr, referenceAmount.ToString("G29"), referenceUnit.Name);
            return result;
        }

        #endregion
    }
}