using Nop.Core.Configuration;

namespace Nop.Plugin.ExchangeRate.EcbExchange
{
    /// <summary>
    /// Represents a EcbExchangeRate plugin settings
    /// </summary>
    public class EcbExchangeRateSettings: ISettings
    {
        /// <summary>
        /// Link to ECB exchange xml data
        /// </summary>
        public string EcbLink { get; set; }
    }
}