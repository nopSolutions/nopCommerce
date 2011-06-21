using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Core.Domain.Directory;
using Nop.Core.Plugins;
using Nop.Services.Directory;

namespace Nop.Services.Tests.Directory
{
    public class TestExchangeRateProvider : BasePlugin, IExchangeRateProvider
    {
        #region Methods

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public IList<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            return new List<ExchangeRate>();
        }

        #endregion

        #region Properties

        public override PluginDescriptor PluginDescriptor
        {
            get
            {
                return new PluginDescriptor()
                {
                    Author = "nopCommerce team",
                    FriendlyName = "Test exchange rate provider",
                    SystemName = "CurrencyExchange.TestProvider",
                    Version = "1.00"
                };
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        
        #endregion
    }
}
