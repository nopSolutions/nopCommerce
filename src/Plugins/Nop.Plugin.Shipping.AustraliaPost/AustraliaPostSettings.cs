using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.AustraliaPost
{
    /// <summary>
    /// Represents settings of Australia Post shipping plugin
    /// </summary>
    public class AustraliaPostSettings : ISettings
    {
        /// <summary>
        /// Gets or sets API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets an amount of the additional handling charge
        /// </summary>
        public decimal AdditionalHandlingCharge { get; set; }
    }
}