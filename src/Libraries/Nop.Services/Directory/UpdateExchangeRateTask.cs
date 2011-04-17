using System;
using System.Xml;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;
using Nop.Services.Directory;
using Nop.Services.Configuration;
using Nop.Core.Domain.Directory;

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
            var a = EngineContext.Current.Resolve<IConfigurationProvider<CurrencySettings>>().Settings;
            if (EngineContext.Current.Resolve<IConfigurationProvider<CurrencySettings>>().Settings.AutoUpdateEnabled)
                return;

            long lastUpdateTimeTicks = EngineContext.Current.Resolve<CurrencySettings>().LastUpdateTime;
            DateTime lastUpdateTime = DateTime.FromBinary(lastUpdateTimeTicks);
            lastUpdateTime = DateTime.SpecifyKind(lastUpdateTime, DateTimeKind.Utc);
            if (lastUpdateTime.AddHours(1) < DateTime.UtcNow)
            {
                //update rates each one hour
                var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
                var exchangeRates = currencyService.GetCurrencyLiveRates(currencyService.GetCurrencyById(EngineContext.Current.Resolve<CurrencySettings>().PrimaryExchangeRateCurrencyId).CurrencyCode);

                foreach (var exchageRate in exchangeRates)
                {
                    Currency currency = EngineContext.Current.Resolve<ICurrencyService>().GetCurrencyByCode(exchageRate.CurrencyCode);
                    if (currency != null)
                    {
                        currency.Rate = exchageRate.Rate;
                        currency.UpdatedOnUtc = DateTime.UtcNow;
                        EngineContext.Current.Resolve<ICurrencyService>().UpdateCurrency(currency);
                    }
                }

                //save new update time value
                EngineContext.Current.Resolve<CurrencySettings>().LastUpdateTime = DateTime.UtcNow.ToBinary();
            }
        }
    }
}
