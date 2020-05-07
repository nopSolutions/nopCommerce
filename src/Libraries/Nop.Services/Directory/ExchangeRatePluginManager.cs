using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Services.Plugins;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Represents an exchange rate plugin manager implementation
    /// </summary>
    public partial class ExchangeRatePluginManager : PluginManager<IExchangeRateProvider>, IExchangeRatePluginManager
    {
        #region Methods

        /// <summary>
        /// Load primary active exchange rate provider
        /// </summary>
        /// <param name="customer">Filter by customer; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>Exchange rate provider</returns>
        public virtual IExchangeRateProvider LoadPrimaryPlugin(Customer customer = null, int storeId = 0)
        {
            var currencySettings = EngineContext.Current.Resolve<CurrencySettings>();

            return LoadPrimaryPlugin(currencySettings.ActiveExchangeRateProviderSystemName, customer, storeId);
        }

        /// <summary>
        /// Check whether the passed exchange rate provider is active
        /// </summary>
        /// <param name="exchangeRateProvider">Exchange rate provider to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IExchangeRateProvider exchangeRateProvider)
        {
            var currencySettings = EngineContext.Current.Resolve<CurrencySettings>();

            return IsPluginActive(exchangeRateProvider, new List<string> { currencySettings.ActiveExchangeRateProviderSystemName });
        }

        #endregion
    }
}