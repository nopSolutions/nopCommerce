using System;
using Nop.Core.Domain.Directory;
using Nop.Services.Tasks;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Represents a task for updating exchange rates
    /// </summary>
    public partial class UpdateExchangeRateTask : ITask
    {
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;

        public UpdateExchangeRateTask(ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            if (!_currencySettings.AutoUpdateEnabled)
                return;

            var primaryCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId).CurrencyCode;
            var exchangeRates = _currencyService.GetCurrencyLiveRates(primaryCurrencyCode);

            foreach (var exchageRate in exchangeRates)
            {
                var currency = _currencyService.GetCurrencyByCode(exchageRate.CurrencyCode);
                if (currency != null)
                {
                    currency.Rate = exchageRate.Rate;
                    currency.UpdatedOnUtc = DateTime.UtcNow;
                    _currencyService.UpdateCurrency(currency);
                }
            }
        }
    }
}
