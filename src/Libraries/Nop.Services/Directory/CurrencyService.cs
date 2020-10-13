using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Stores;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Currency service
    /// </summary>
    public partial class CurrencyService : ICurrencyService
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IExchangeRatePluginManager _exchangeRatePluginManager;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public CurrencyService(CurrencySettings currencySettings,
            IExchangeRatePluginManager exchangeRatePluginManager,
            IRepository<Currency> currencyRepository,
            IStoreMappingService storeMappingService)
        {
            _currencySettings = currencySettings;
            _exchangeRatePluginManager = exchangeRatePluginManager;
            _currencyRepository = currencyRepository;
            _storeMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        #region Currency

        /// <summary>
        /// Deletes currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual async Task DeleteCurrency(Currency currency)
        {
            await _currencyRepository.Delete(currency);
        }

        /// <summary>
        /// Gets a currency
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        /// <returns>Currency</returns>
        public virtual async Task<Currency> GetCurrencyById(int currencyId)
        {
            return await _currencyRepository.GetById(currencyId, cache => default);
        }

        /// <summary>
        /// Gets a currency by code
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>Currency</returns>
        public virtual async Task<Currency> GetCurrencyByCode(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode))
                return null;

            return (await GetAllCurrencies(true))
                .FirstOrDefault(c => c.CurrencyCode.ToLower() == currencyCode.ToLower());
        }

        /// <summary>
        /// Gets all currencies
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Currencies</returns>
        public virtual async Task<IList<Currency>> GetAllCurrencies(bool showHidden = false, int storeId = 0)
        {
            var currencies = await _currencyRepository.GetAll(query =>
            {
                if (!showHidden)
                    query = query.Where(c => c.Published);

                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id);

                return query;
            }, cache => cache.PrepareKeyForDefaultCache(NopDirectoryDefaults.CurrenciesAllCacheKey, showHidden));

            //store mapping
            if (storeId > 0)
                currencies = currencies
                    .Where(c => _storeMappingService.Authorize(c, storeId).Result)
                    .ToList();

            return currencies;
        }

        /// <summary>
        /// Inserts a currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual async Task InsertCurrency(Currency currency)
        {
            await _currencyRepository.Insert(currency);
        }

        /// <summary>
        /// Updates the currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual async Task UpdateCurrency(Currency currency)
        {
            await _currencyRepository.Update(currency);
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Gets live rates regarding the passed currency
        /// </summary>
        /// <param name="currencyCode">Currency code; pass null to use primary exchange rate currency</param>
        /// <returns>Exchange rates</returns>
        public virtual async Task<IList<ExchangeRate>> GetCurrencyLiveRates(string currencyCode = null)
        {
            var exchangeRateProvider = _exchangeRatePluginManager.LoadPrimaryPlugin()
                ?? throw new Exception("Active exchange rate provider cannot be loaded");

            currencyCode ??= (await GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId))?.CurrencyCode
                ?? throw new NopException("Primary exchange rate currency is not set");

            return await exchangeRateProvider.GetCurrencyLiveRates(currencyCode);
        }

        /// <summary>
        /// Converts currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="exchangeRate">Currency exchange rate</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertCurrency(decimal amount, decimal exchangeRate)
        {
            if (amount != decimal.Zero && exchangeRate != decimal.Zero)
                return amount * exchangeRate;

            return decimal.Zero;
        }

        /// <summary>
        /// Converts currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public virtual async Task<decimal> ConvertCurrency(decimal amount, Currency sourceCurrencyCode, Currency targetCurrencyCode)
        {
            if (sourceCurrencyCode == null)
                throw new ArgumentNullException(nameof(sourceCurrencyCode));

            if (targetCurrencyCode == null)
                throw new ArgumentNullException(nameof(targetCurrencyCode));

            var result = amount;
            
            if (result == decimal.Zero || sourceCurrencyCode.Id == targetCurrencyCode.Id)
                return result;

            result = await ConvertToPrimaryExchangeRateCurrency(result, sourceCurrencyCode);
            result = await ConvertFromPrimaryExchangeRateCurrency(result, targetCurrencyCode);
            return result;
        }

        /// <summary>
        /// Converts to primary exchange rate currency 
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <returns>Converted value</returns>
        public virtual async Task<decimal> ConvertToPrimaryExchangeRateCurrency(decimal amount, Currency sourceCurrencyCode)
        {
            if (sourceCurrencyCode == null)
                throw new ArgumentNullException(nameof(sourceCurrencyCode));

            var primaryExchangeRateCurrency = await GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId);
            if (primaryExchangeRateCurrency == null)
                throw new Exception("Primary exchange rate currency cannot be loaded");

            var result = amount;
            if (result == decimal.Zero || sourceCurrencyCode.Id == primaryExchangeRateCurrency.Id)
                return result;

            var exchangeRate = sourceCurrencyCode.Rate;
            if (exchangeRate == decimal.Zero)
                throw new NopException($"Exchange rate not found for currency [{sourceCurrencyCode.Name}]");
            result = result / exchangeRate;

            return result;
        }

        /// <summary>
        /// Converts from primary exchange rate currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public virtual async Task<decimal> ConvertFromPrimaryExchangeRateCurrency(decimal amount, Currency targetCurrencyCode)
        {
            if (targetCurrencyCode == null)
                throw new ArgumentNullException(nameof(targetCurrencyCode));

            var primaryExchangeRateCurrency = await GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId);
            if (primaryExchangeRateCurrency == null)
                throw new Exception("Primary exchange rate currency cannot be loaded");

            var result = amount;
            if (result == decimal.Zero || targetCurrencyCode.Id == primaryExchangeRateCurrency.Id)
                return result;

            var exchangeRate = targetCurrencyCode.Rate;
            if (exchangeRate == decimal.Zero)
                throw new NopException($"Exchange rate not found for currency [{targetCurrencyCode.Name}]");
            result = result * exchangeRate;

            return result;
        }

        /// <summary>
        /// Converts to primary store currency 
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <returns>Converted value</returns>
        public virtual async Task<decimal> ConvertToPrimaryStoreCurrency(decimal amount, Currency sourceCurrencyCode)
        {
            if (sourceCurrencyCode == null)
                throw new ArgumentNullException(nameof(sourceCurrencyCode));

            var primaryStoreCurrency = await GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            var result = await ConvertCurrency(amount, sourceCurrencyCode, primaryStoreCurrency);
            
            return result;
        }

        /// <summary>
        /// Converts from primary store currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public virtual async Task<decimal> ConvertFromPrimaryStoreCurrency(decimal amount, Currency targetCurrencyCode)
        {
            var primaryStoreCurrency = await GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            var result = await ConvertCurrency(amount, primaryStoreCurrency, targetCurrencyCode);
            
            return result;
        }

        #endregion

        #endregion
    }
}