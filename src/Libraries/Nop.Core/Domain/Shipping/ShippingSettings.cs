
using Nop.Core.Configuration;
using System.Collections.Generic;

namespace Nop.Core.Domain.Shipping
{
    public class ShippingSettings : ISettings
    {
        /// <summary>
        /// Gets or sets an system names of active shipping rate computation methods
        /// </summary>
        public List<string> ActiveShippingRateComputationMethodSystemNames { get; set; }
    }
}