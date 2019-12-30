using Nop.Core.Configuration;

namespace Nop.Core.Domain
{
    public partial class CachingSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether caching of customer roles is enabled
        /// </summary>
        public bool CachingCustomerRolesEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether caching of customer address is enabled
        /// </summary>
        public bool CachingCustomerAddressEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether caching of shopping cart is enabled
        /// </summary>
        public bool CachingShoppingCartEnabled { get; set; } = true;
    }
}
