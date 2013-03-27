using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Stores;
using Nop.Core.Plugins;
using Nop.Services.Customers;
using Nop.Services.Events;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Currency service
    /// </summary>
    public partial class CurrencyService : ICurrencyService
    {
        #region Constants

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// {1} : store ID
        /// </remarks>
        private const string CURRENCIES_ALL_KEY = "Nop.currency.all-{0}-{1}";
        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string CURRENCIES_PATTERN_KEY = "Nop.currency.";

        #endregion

        #region Fields

        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly ICacheManager _cacheManager;
        private readonly CurrencySettings _currencySettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="currencyRepository">Currency repository</param>
        /// <param name="storeMappingRepository">Store mapping repository</param>
        /// <param name="currencySettings">Currency settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="eventPublisher">Event published</param>
        public CurrencyService(ICacheManager cacheManager,
            IRepository<Currency> currencyRepository,
            IRepository<StoreMapping> storeMappingRepository,
            CurrencySettings currencySettings,
            IPluginFinder pluginFinder,
            IEventPublisher eventPublisher)
        {
            this._cacheManager = cacheManager;
            this._currencyRepository = currencyRepository;
            this._storeMappingRepository = storeMappingRepository;
            this._currencySettings = currencySettings;
            this._pluginFinder = pluginFinder;
            this._eventPublisher = eventPublisher;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public virtual IList<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            var exchangeRateProvider = LoadActiveExchangeRateProvider();
            if (exchangeRateProvider == null)
                throw new Exception("Active exchange rate provider cannot be loaded");
            return exchangeRateProvider.GetCurrencyLiveRates(exchangeRateCurrencyCode);
        }

        /// <summary>
        /// Deletes currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void DeleteCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");
            
            _currencyRepository.Delete(currency);

            _cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(currency);
        }

        /// <summary>
        /// Gets a currency
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        /// <returns>Currency</returns>
        public virtual Currency GetCurrencyById(int currencyId)
        {
            if (currencyId == 0)
                return null;

            return _currencyRepository.GetById(currencyId);
        }

        /// <summary>
        /// Gets a currency by code
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>Currency</returns>
        public virtual Currency GetCurrencyByCode(string currencyCode)
        {
            if (String.IsNullOrEmpty(currencyCode))
                return null;
            return GetAllCurrencies(true).FirstOrDefault(c => c.CurrencyCode.ToLower() == currencyCode.ToLower());
        }

        /// <summary>
        /// Gets all currencies
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="storeId">Load records allows only in specified store; pass 0 to load all records</param>
        /// <returns>Currencies</returns>
        public virtual IList<Currency> GetAllCurrencies(bool showHidden = false, int storeId = 0)
        {
            string key = string.Format(CURRENCIES_ALL_KEY, showHidden, storeId);
            return _cacheManager.Get(key, () =>
            {
                var query = _currencyRepository.Table;
                if (!showHidden)
                    query = query.Where(c => c.Published);
                query = query.OrderBy(c => c.DisplayOrder);

                //Store mapping
                if (storeId > 0)
                {
                    query = from c in query
                            join sm in _storeMappingRepository.Table on c.Id equals sm.EntityId into c_sm
                            from sm in c_sm.DefaultIfEmpty()
                            where !c.LimitedToStores || (sm.EntityName == "Currency" && storeId == sm.StoreId)
                            select c;

                    //only distinct languages (group by ID)
                    query = from c in query
                            group c by c.Id
                            into cGroup
                            orderby cGroup.Key
                            select cGroup.FirstOrDefault();
                    query = query.OrderBy(c => c.DisplayOrder);
                }

                var currencies = query.ToList();
                return currencies;
            });
        }

        /// <summary>
        /// Inserts a currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void InsertCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");

            _currencyRepository.Insert(currency);

            _cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(currency);
        }

        /// <summary>
        /// Updates the currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void UpdateCurrency(Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException("currency");

            _currencyRepository.Update(currency);

            _cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(currency);
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
        public virtual decimal ConvertCurrency(decimal amount, Currency sourceCurrencyCode, Currency targetCurrencyCode)
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
        public virtual decimal ConvertToPrimaryExchangeRateCurrency(decimal amount, Currency sourceCurrencyCode)
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
        public virtual decimal ConvertFromPrimaryExchangeRateCurrency(decimal amount, Currency targetCurrencyCode)
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

        /// <summary>
        /// Converts to primary store currency 
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertToPrimaryStoreCurrency(decimal amount, Currency sourceCurrencyCode)
        {
            decimal result = amount;
            var primaryStoreCurrency = GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            if (result != decimal.Zero && sourceCurrencyCode.Id != primaryStoreCurrency.Id)
            {
                decimal exchangeRate = sourceCurrencyCode.Rate;
                if (exchangeRate == decimal.Zero)
                    throw new NopException(string.Format("Exchange rate not found for currency [{0}]", sourceCurrencyCode.Name));
                result = result / exchangeRate;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary store currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertFromPrimaryStoreCurrency(decimal amount, Currency targetCurrencyCode)
        {
            decimal result = amount;
            var primaryStoreCurrency = GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);
            result = ConvertCurrency(amount, primaryStoreCurrency, targetCurrencyCode);
            return result;
        }
       

        /// <summary>
        /// Load active exchange rate provider
        /// </summary>
        /// <returns>Active exchange rate provider</returns>
        public virtual IExchangeRateProvider LoadActiveExchangeRateProvider()
        {
            var exchangeRateProvider = LoadExchangeRateProviderBySystemName(_currencySettings.ActiveExchangeRateProviderSystemName);
            if (exchangeRateProvider == null)
                exchangeRateProvider = LoadAllExchangeRateProviders().FirstOrDefault();
            return exchangeRateProvider;
        }

        /// <summary>
        /// Load exchange rate provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found exchange rate provider</returns>
        public virtual IExchangeRateProvider LoadExchangeRateProviderBySystemName(string systemName)
        {
            var descriptor = _pluginFinder.GetPluginDescriptorBySystemName<IExchangeRateProvider>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IExchangeRateProvider>();

            return null;
        }

        /// <summary>
        /// Load all exchange rate providers
        /// </summary>
        /// <returns>Exchange rate providers</returns>
        public virtual IList<IExchangeRateProvider> LoadAllExchangeRateProviders()
        {
            var exchangeRateProviders = _pluginFinder.GetPlugins<IExchangeRateProvider>();
            return exchangeRateProviders
                .OrderBy(tp => tp.PluginDescriptor)
                .ToList();
        }
        #endregion
    }
}