using System;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Tasks;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Represents a task for updating exchange rates
    /// </summary>
    public partial class UpdateExchangeRateTask : ITask
    {
        private readonly ICurrencyService _currencyService;
        private readonly ISettingService _settingService;
        private readonly CurrencySettings _currencySettings;

        public UpdateExchangeRateTask(ICurrencyService currencyService, 
            ISettingService settingService, CurrencySettings currencySettings)
        {
            this._currencyService = currencyService;
            this._settingService = settingService;
            this._currencySettings = currencySettings;
        }

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            if (!_currencySettings.AutoUpdateEnabled)
                return;

            long lastUpdateTimeTicks = _currencySettings.LastUpdateTime;
            DateTime lastUpdateTime = DateTime.FromBinary(lastUpdateTimeTicks);
            lastUpdateTime = DateTime.SpecifyKind(lastUpdateTime, DateTimeKind.Utc);
            if (lastUpdateTime.AddHours(1) < DateTime.UtcNow)
            {
                //update rates each one hour
                var exchangeRates = _currencyService.GetCurrencyLiveRates(_currencyService.GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId).CurrencyCode);

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

                //save new update time value
                _currencySettings.LastUpdateTime = DateTime.UtcNow.ToBinary();
                _settingService.SaveSetting(_currencySettings);
            }
        }
    }
}
