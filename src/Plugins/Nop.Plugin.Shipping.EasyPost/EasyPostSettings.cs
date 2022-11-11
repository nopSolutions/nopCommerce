using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.EasyPost
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class EasyPostSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the test API key
        /// </summary>
        public string TestApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the sandbox testing environment
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use all available carrier accounts to get shipping rates
        /// </summary>
        public bool UseAllAvailableCarriers { get; set; }

        /// <summary>
        /// Gets or sets the list of carrier accounts whose rates to use 
        /// </summary>
        public List<string> CarrierAccounts { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether to use the address verification.
        /// Verification system checks the address and will make minor corrections to spelling/format if applicable
        /// </summary>
        public bool AddressVerification { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the strict address verification.
        /// The failure of this verification causes the whole request to fail
        /// </summary>
        public bool StrictAddressVerification { get; set; }

        /// <summary>
        /// Gets or sets a webhook URL
        /// </summary>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to log shipment messages. 
        /// If there will be many, they can be a little annoying, so let's add a way to disable logging
        /// </summary>
        public bool LogShipmentMessages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SmartRate feature
        /// </summary>
        public bool UseSmartRates { get; set; }
    }
}