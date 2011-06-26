using System;
using System.Xml;
using Nop.Core.Configuration;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;
using Nop.Services.Configuration;

namespace Nop.Services.Directory
{
    /// <summary>
    /// Represents a task for updating exchange rates
    /// </summary>
    public partial class UpdateExchangeRateTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            var currencySettings = EngineContext.Current.Resolve<IConfigurationProvider<CurrencySettings>>().Settings;
            if (!currencySettings.AutoUpdateEnabled)
                return;

            long lastUpdateTimeTicks = currencySettings.LastUpdateTime;
            DateTime lastUpdateTime = DateTime.FromBinary(lastUpdateTimeTicks);
            lastUpdateTime = DateTime.SpecifyKind(lastUpdateTime, DateTimeKind.Utc);
            if (lastUpdateTime.AddHours(1) < DateTime.UtcNow)
            {
                //update rates each one hour
                var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
                var exchangeRates = currencyService.GetCurrencyLiveRates(currencyService.GetCurrencyById(currencySettings.PrimaryExchangeRateCurrencyId).CurrencyCode);

                foreach (var exchageRate in exchangeRates)
                {
                    var currency = currencyService.GetCurrencyByCode(exchageRate.CurrencyCode);
                    if (currency != null)
                    {
                        currency.Rate = exchageRate.Rate;
                        currency.UpdatedOnUtc = DateTime.UtcNow;
                        currencyService.UpdateCurrency(currency);
                    }
                }

                //save new update time value
                currencySettings.LastUpdateTime = DateTime.UtcNow.ToBinary();
                var settingService = EngineContext.Current.Resolve<ISettingService>();
                settingService.SaveSetting(currencySettings);
            }
        }
    }
}
