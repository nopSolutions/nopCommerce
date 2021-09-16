using Nop.Core.Configuration;

namespace Nop.Plugin.ExchangeRate.EcbExchange
{
    public class EcbExchangeRateSettings: ISettings
    {
        public string EcbLink { get; set; }
    }
}