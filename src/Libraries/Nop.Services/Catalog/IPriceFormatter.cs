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

using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;

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
        /// <returns>Price</returns>
        string FormatPrice(decimal price);

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Price</returns>
        string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency);

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        string FormatPrice(decimal price, bool showCurrency, bool showTax);

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        string FormatPrice(decimal price, bool showCurrency, 
            string currencyCode, bool showTax);

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        string FormatPrice(decimal price, bool showCurrency,
            string currencyCode, Language language, bool priceIncludesTax);

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        string FormatPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax);

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
        string FormatPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax);


        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>Price</returns>
        string FormatShippingPrice(decimal price, bool showCurrency);

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        string FormatShippingPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax);
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
        string FormatShippingPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax);
        
        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        string FormatShippingPrice(decimal price, bool showCurrency, 
            string currencyCode, Language language, bool priceIncludesTax);



        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>Price</returns>
        string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency);

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency,
            Currency targetCurrency, Language language, bool priceIncludesTax);

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
        string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax);

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, 
            string currencyCode, Language language, bool priceIncludesTax);



        /// <summary>
        /// Formats a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Formatted tax rate</returns>
        string FormatTaxRate(decimal taxRate);
    }
}
