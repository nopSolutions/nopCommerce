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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Directory.ExchangeRates;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Directory
{
    /// <summary>
    /// Currency manager
    /// </summary>
    public partial class CurrencyManager : ICurrencyManager
    {
        #region Constants
        private const string CURRENCIES_ALL_KEY = "Nop.currency.all-{0}";
        private const string CURRENCIES_BY_ID_KEY = "Nop.currency.id-{0}";
        private const string CURRENCIES_PATTERN_KEY = "Nop.currency.";
        #endregion

        #region Methods

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public List<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            var exchangeRateProvider = IoCFactory.Resolve<ICurrencyManager>().CurrentExchangeRateProvider;
            return exchangeRateProvider.GetCurrencyLiveRates(exchangeRateCurrencyCode);
        }

        /// <summary>
        /// Deletes currency
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        public void DeleteCurrency(int currencyId)
        {
            var currency = GetCurrencyById(currencyId);
            if (currency == null)
                return;

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(currency))
                context.Currencies.Attach(currency);
            context.DeleteObject(currency);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CURRENCIES_PATTERN_KEY);
            }
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
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (Currency)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Currencies
                        where c.CurrencyId == currencyId
                        select c;
            var currency = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, currency);
            }
            return currency;
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
        /// <returns>Currency collection</returns>
        public List<Currency> GetAllCurrencies()
        {
            bool showHidden = NopContext.Current.IsAdmin;
            return GetAllCurrencies(showHidden);
        }

        /// <summary>
        /// Gets all currencies
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Currency collection</returns>
        public List<Currency> GetAllCurrencies(bool showHidden)
        {
            string key = string.Format(CURRENCIES_ALL_KEY, showHidden);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<Currency>)obj2;
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            var query = from c in context.Currencies
                        orderby c.DisplayOrder
                        where showHidden || c.Published
                        select c;
            var currencies = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, currencies);
            }
            return currencies;
        }

        /// <summary>
        /// Inserts a currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public void InsertCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");
            
            currency.Name = CommonHelper.EnsureNotNull(currency.Name);
            currency.Name = CommonHelper.EnsureMaximumLength(currency.Name, 50);
            currency.CurrencyCode = CommonHelper.EnsureNotNull(currency.CurrencyCode);
            currency.CurrencyCode = CommonHelper.EnsureMaximumLength(currency.CurrencyCode, 5);
            currency.DisplayLocale = CommonHelper.EnsureNotNull(currency.DisplayLocale);
            currency.DisplayLocale = CommonHelper.EnsureMaximumLength(currency.DisplayLocale, 50);
            currency.CustomFormatting = CommonHelper.EnsureNotNull(currency.CustomFormatting);
            currency.CustomFormatting = CommonHelper.EnsureMaximumLength(currency.CustomFormatting, 50);

            try
            {
                CultureInfo ci = CultureInfo.GetCultureInfo(currency.DisplayLocale);
            }
            catch (Exception)
            {
                throw new NopException("Specified display locale culture is not supported");
            }

            var context = ObjectContextHelper.CurrentObjectContext;

            context.Currencies.AddObject(currency);
            context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CURRENCIES_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public void UpdateCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");

            currency.Name = CommonHelper.EnsureNotNull(currency.Name);
            currency.Name = CommonHelper.EnsureMaximumLength(currency.Name, 50);
            currency.CurrencyCode = CommonHelper.EnsureNotNull(currency.CurrencyCode);
            currency.CurrencyCode = CommonHelper.EnsureMaximumLength(currency.CurrencyCode, 5);
            currency.DisplayLocale = CommonHelper.EnsureNotNull(currency.DisplayLocale);
            currency.DisplayLocale = CommonHelper.EnsureMaximumLength(currency.DisplayLocale, 50);
            currency.CustomFormatting = CommonHelper.EnsureNotNull(currency.CustomFormatting);
            currency.CustomFormatting = CommonHelper.EnsureMaximumLength(currency.CustomFormatting, 50);

            try
            {
                CultureInfo ci = CultureInfo.GetCultureInfo(currency.DisplayLocale);
            }
            catch (Exception)
            {
                throw new NopException("Specified display locale culture is not supported");
            }

            var context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(currency))
                context.Currencies.Attach(currency);

            context.SaveChanges();


            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CURRENCIES_PATTERN_KEY);
            }
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
            if (sourceCurrencyCode.CurrencyId == targetCurrencyCode.CurrencyId)
                return result;
            if (result != decimal.Zero && sourceCurrencyCode.CurrencyId != targetCurrencyCode.CurrencyId)
            {
                result = ConvertToPrimaryExchangeRateCurrency(result, sourceCurrencyCode);
                result = ConvertFromPrimaryExchangeRateCurrency(result, targetCurrencyCode);
            }
            result = Math.Round(result, 2);
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
            if (result != decimal.Zero && sourceCurrencyCode.CurrencyId != IoCFactory.Resolve<ICurrencyManager>().PrimaryExchangeRateCurrency.CurrencyId)
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
            if (result != decimal.Zero && targetCurrencyCode.CurrencyId != IoCFactory.Resolve<ICurrencyManager>().PrimaryExchangeRateCurrency.CurrencyId)
            {
                decimal exchangeRate = targetCurrencyCode.Rate;
                if (exchangeRate == decimal.Zero)
                    throw new NopException(string.Format("Exchange rate not found for currency [{0}]", targetCurrencyCode.Name));
                result = result * exchangeRate;
            }
            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.CurrencyManager.CacheEnabled");
            }
        }

        /// <summary>
        /// Gets or sets a primary store currency
        /// </summary>
        public Currency PrimaryStoreCurrency
        {
            get
            {
                int primaryStoreCurrencyId = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Currency.PrimaryStoreCurrency");
                return GetCurrencyById(primaryStoreCurrencyId);
            }
            set
            {
                if (value != null)
                    IoCFactory.Resolve<ISettingManager>().SetParam("Currency.PrimaryStoreCurrency", value.CurrencyId.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a primary exchange rate currency
        /// </summary>
        public Currency PrimaryExchangeRateCurrency
        {
            get
            {
                int primaryExchangeRateCurrencyId = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("Currency.PrimaryExchangeRateCurrency");
                return GetCurrencyById(primaryExchangeRateCurrencyId);
            }
            set
            {
                if (value != null)
                    IoCFactory.Resolve<ISettingManager>().SetParam("Currency.PrimaryExchangeRateCurrency", value.CurrencyId.ToString());
            }
        }

        /// <summary>
        /// Gets a current exchange rate provider
        /// </summary>
        public IExchangeRateProvider CurrentExchangeRateProvider
        {
            get
            {
                int i = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("ExchangeRateProvider.Current");
                string className = IoCFactory.Resolve<ISettingManager>().GetSettingValue(String.Format("ExchangeRateProvider{0}.Classname", i));

                if (String.IsNullOrEmpty(className))
                {
                    throw new NopException("Current exchange rate provider class name isn't valid");
                }

                Type type = Type.GetType(className);

                if (type == null)
                {
                    throw new NopException("Current exchange rate provider type isn't valid");
                }

                IExchangeRateProvider instance = Activator.CreateInstance(type) as IExchangeRateProvider;
                if (instance == null)
                {
                    throw new NopException("Current exchange rate provider isn't valid");
                }
                return instance;
            }
        }
        #endregion
    }
}