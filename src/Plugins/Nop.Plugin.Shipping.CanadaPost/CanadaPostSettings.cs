using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.CanadaPost
{
    /// <summary>
    /// Represents settings of Canada Post shipping plugin
    /// </summary>
    public class CanadaPostSettings : ISettings
    {
        public CanadaPostSettings()
        {
            SelectedServicesCodes = new List<string>();
        }

        /// <summary>
        /// Gets or sets customer number
        /// </summary>
        public string CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets contract identifier
        /// </summary>
        public string ContractId { get; set; }

        /// <summary>
        /// Gets or sets the API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether to use sandbox (testing environment)
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a list of codes of selected shipping services
        /// </summary>
        public List<string> SelectedServicesCodes { get; set; }
    }
}