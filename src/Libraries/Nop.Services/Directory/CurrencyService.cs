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
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Directory;
using Nop.Data;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Currency service
    /// </summary>
    public partial class CurrencyService : ICurrencyService
    {
        #region Constants
        private const string CURRENCIES_ALL_KEY = "Nop.currency.all-{0}";
        private const string CURRENCIES_BY_ID_KEY = "Nop.currency.id-{0}";
        private const string CURRENCIES_PATTERN_KEY = "Nop.currency.";
        #endregion

        #region Fields

        private readonly IRepository<Currency> _currencyRepository;
        private readonly ICacheManager _cacheManager;
        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="currencyRepository">Currency repository</param>
        /// <param name="currencySettings">Currency settings</param>
        public CurrencyService(ICacheManager cacheManager,
            IRepository<Currency> currencyRepository,
            CurrencySettings currencySettings)
        {
            this._cacheManager = cacheManager;
            this._currencyRepository = currencyRepository;
            this._currencySettings = currencySettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public void DeleteCurrency(Currency currency)
        {
            if (currency == null)
                return;

            //TODO load all customers (currency.Customers property) and set new currency to them

            _currencyRepository.Delete(currency);

            _cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a currency
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        /// <returns>Currency</returns>
        public Currency GetCurrencyById(int currencyId)
        {
            if (currencyId == 0)
                return null;

            string key = string.Format(CURRENCIES_BY_ID_KEY, currencyId);
            return _cacheManager.Get(key, () =>
            {
                return _currencyRepository.GetById(currencyId);
            });
        }

        /// <summary>
        /// Gets a currency by code
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>Currency</returns>
        public Currency GetCurrencyByCode(string currencyCode)
        {
            if (String.IsNullOrEmpty(currencyCode))
                return null;
            return GetAllCurrencies(true).FirstOrDefault(c => c.CurrencyCode.ToLower() == currencyCode.ToLower());
        }

        /// <summary>
        /// Gets all currencies
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Currency collection</returns>
        public IList<Currency> GetAllCurrencies(bool showHidden = false)
        {
            string key = string.Format(CURRENCIES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _currencyRepository.Table
                            orderby c.DisplayOrder
                            where showHidden || c.Published
                            select c;
                var currencies = query.ToList();
                return currencies;
            });
        }

        /// <summary>
        /// Inserts a currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public void InsertCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");

            _currencyRepository.Insert(currency);

            _cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public void UpdateCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");

            _currencyRepository.Update(currency);

            _cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);
        }

        /// <summary>
        /// Converts currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public decimal ConvertCurrency(decimal amount, Currency sourceCurrencyCode,
            Currency targetCurrencyCode)
        {
            decimal result = amount;
            if (sourceCurrencyCode.Id == targetCurrencyCode.Id)
                return result;
            if (result != decimal.Zero && sourceCurrencyCode.Id != targetCurrencyCode.Id)
            {
                result = ConvertToPrimaryExchangeRateCurrency(result, sourceCurrencyCode);
                result = ConvertFromPrimaryExchangeRateCurrency(result, targetCurrencyCode);
            }
            return result;
        }

        /// <summary>
        /// Converts to primary exchange rate currency 
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <returns>Converted value</returns>
        public decimal ConvertToPrimaryExchangeRateCurrency(decimal amount,
            Currency sourceCurrencyCode)
        {
            decimal result = amount;
            var primaryExchangeRateCurrency = GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId);
            if (result != decimal.Zero && sourceCurrencyCode.Id != primaryExchangeRateCurrency.Id)
            {
                decimal exchangeRate = sourceCurrencyCode.Rate;
                if (exchangeRate == decimal.Zero)
                    throw new NopException(string.Format("Exchange rate not found for currency [{0}]", sourceCurrencyCode.Name));
                result = result / exchangeRate;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary exchange rate currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public decimal ConvertFromPrimaryExchangeRateCurrency(decimal amount,
            Currency targetCurrencyCode)
        {
            decimal result = amount;
            var primaryExchangeRateCurrency = GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId);
            if (result != decimal.Zero && targetCurrencyCode.Id != primaryExchangeRateCurrency.Id)
            {
                decimal exchangeRate = targetCurrencyCode.Rate;
                if (exchangeRate == decimal.Zero)
                    throw new NopException(string.Format("Exchange rate not found for currency [{0}]", targetCurrencyCode.Name));
                result = result * exchangeRate;
            }
            return result;
        }
        #endregion
    }
}