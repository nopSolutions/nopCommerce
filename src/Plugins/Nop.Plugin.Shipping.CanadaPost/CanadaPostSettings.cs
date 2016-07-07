
using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.CanadaPost
{
    public class CanadaPostSettings : ISettings
    {
        /// <summary>
        /// Gets or sets customer number
        /// </summary>
        public string CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets the API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }
    }
}