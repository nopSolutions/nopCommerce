using System.Collections.Generic;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Shipping
{
    public class ShippingSettings : ISettings
    {
        public ShippingSettings()
        {
            ActiveShippingRateComputationMethodSystemNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets an system names of active shipping rate computation methods
        /// </summary>
        public List<string> ActiveShippingRateComputationMethodSystemNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Free shipping over X' is enabled
        /// </summary>
        public bool FreeShippingOverXEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value of 'Free shipping over X' option
        /// </summary>
        public decimal FreeShippingOverXValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 'Estimate shipping' option is enabled
        /// </summary>
        public bool EstimateShippingEnabled { get; set; }

        /// <summary>
        /// Gets or sets shipping origin address
        /// </summary>
        public int ShippingOriginAddressId { get; set; }
    }
}