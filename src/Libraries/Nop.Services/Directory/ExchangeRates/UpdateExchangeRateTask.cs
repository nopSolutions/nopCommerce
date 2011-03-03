//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Xml;
using Nop.Core.Infrastructure;
using Nop.Core.Tasks;
using Nop.Services.Directory;
using Nop.Services.Configuration;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Directory.ExchangeRates
{
    /// <summary>
    /// Represents a task for uupdating exchange rates
    /// </summary>
    public partial class UpdateExchangeRateTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        /// <param name="node">Xml node that represents a task description</param>
        public void Execute(XmlNode node)
        {
            if (EngineContext.Current.Resolve<ISettingService>().GetSettingByKey<string>("ExchangeRateProvider.AutoUpdateEnabled")!=null)
                return;

            long lastUpdateTimeTicks = EngineContext.Current.Resolve<ISettingService>().GetSettingByKey<int>("ExchangeRateProvider.LastUpdateTime", 0);
            DateTime lastUpdateTime = DateTime.FromBinary(lastUpdateTimeTicks);
            lastUpdateTime = DateTime.SpecifyKind(lastUpdateTime, DateTimeKind.Utc);
            if (lastUpdateTime.AddHours(1) < DateTime.UtcNow)
            {
                //update rates each one hour

                var exchangeRates = EngineContext.Current.Resolve<ICurrencyService>().GetCurrencyLiveRates(EngineContext.Current.Resolve<ICurrencyService>().PrimaryExchangeRateCurrency.CurrencyCode);

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
                EngineContext.Current.Resolve<ISettingService>().SetSetting<string>("ExchangeRateProvider.LastUpdateTime", DateTime.UtcNow.ToBinary().ToString());
            }
        }
    }
}
