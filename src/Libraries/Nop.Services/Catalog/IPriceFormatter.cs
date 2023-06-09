using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Price formatter
    /// </summary>
    public partial interface IPriceFormatter
    {
        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        Task<string> FormatPriceAsync(decimal price);

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
        Task<string> FormatPriceAsync(decimal price, bool showCurrency, Currency targetCurrency);

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
        Task<string> FormatPriceAsync(decimal price, bool showCurrency, bool showTax);

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
        Task<string> FormatPriceAsync(decimal price, bool showCurrency,
            string currencyCode, bool showTax, int languageId);

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
        Task<string> FormatOrderPriceAsync(decimal price,
            decimal currencyRate, string customerCurrencyCode, bool displayCustomerCurrency,
            Currency primaryStoreCurrency, int languageId, bool? priceIncludesTax = null, bool? showTax = null);

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
        Task<string> FormatPriceAsync(decimal price, bool showCurrency,
            string currencyCode, int languageId, bool priceIncludesTax);

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
        Task<string> FormatPriceAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax);

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
        Task<string> FormatPriceAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax, bool showTax);

        /// <summary>
        /// Formats the price of rental product (with rental period)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rental product price with period
        /// </returns>
        Task<string> FormatRentalProductPeriodAsync(Product product, string price);

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency);

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
        Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax);

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
        Task<string> FormatShippingPriceAsync(decimal price, bool showCurrency,
            string currencyCode, int languageId, bool priceIncludesTax);

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price
        /// </returns>
        Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency);

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
        Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency,
            Currency targetCurrency, int languageId, bool priceIncludesTax);

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
        Task<string> FormatPaymentMethodAdditionalFeeAsync(decimal price, bool showCurrency,
            string currencyCode, int languageId, bool priceIncludesTax);

        /// <summary>
        /// Formats a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Formatted tax rate</returns>
        string FormatTaxRate(decimal taxRate);

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
        Task<string> FormatBasePriceAsync(Product product, decimal? productPrice, decimal? totalWeight = null);
    }
}