
using System.Collections.Generic;
using Nop.Core.Domain;
using Nop.Core.Domain.Directory;
using Nop.Core.Plugins;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Exchange rate provider interface
    /// </summary>
    public partial interface IExchangeRateProvider : IPlugin
    {
        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        List<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode);
    }
}