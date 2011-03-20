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
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get
            {
                return "Test exchange rate provider";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public override string SystemName
        {
            get
            {
                return "CurrencyExchange.TestProvider";
            }
        }

        #endregion
    }
}
