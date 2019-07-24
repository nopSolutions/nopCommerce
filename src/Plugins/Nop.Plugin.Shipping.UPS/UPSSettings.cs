using Nop.Core.Configuration;
using Nop.Plugin.Shipping.UPS.Domain;

namespace Nop.Plugin.Shipping.UPS
{
    /// <summary>
    /// Represents settings of the UPS shipping plugin
    /// </summary>
    public class UPSSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the account number
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the access key
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets an amount of the additional handling charge
        /// </summary>
        public decimal AdditionalHandlingCharge { get; set; }

        /// <summary>
        /// Gets or sets UPS customer classification
        /// </summary>
        public CustomerClassification CustomerClassification { get; set; }

        /// <summary>
        /// Gets or sets a pickup type
        /// </summary>
        public PickupType PickupType { get; set; }

        /// <summary>
        /// Gets or sets packaging type
        /// </summary>
        public PackagingType PackagingType { get; set; }

        /// <summary>
        /// Gets or sets offered carrier services
        /// </summary>
        public string CarrierServicesOffered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Saturday Delivery enabled
        /// </summary>
        public bool SaturdayDeliveryEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to insure packages
        /// </summary>
        public bool InsurePackage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to pass package dimensions
        /// </summary>
        public bool PassDimensions { get; set; }

        /// <summary>
        /// Gets or sets the packing package volume
        /// </summary>
        public int PackingPackageVolume { get; set; }

        /// <summary>
        /// Gets or sets packing type
        /// </summary>
        public PackingType PackingType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to record plugin tracing in log
        /// </summary>
        public bool Tracing { get; set; }
    }
}