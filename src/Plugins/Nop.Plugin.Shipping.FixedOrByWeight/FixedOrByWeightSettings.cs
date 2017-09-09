using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.FixedOrByWeight
{
    /// <summary>
    /// Represents settings of the "Fixed or by weight" shipping plugin
    /// </summary>
    public class FixedOrByWeightSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to limit shipping methods to configured ones
        /// </summary>
        public bool LimitMethodsToCreated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the "shipping calculation by weight" method is selected
        /// </summary>
        public bool ShippingByWeightEnabled { get; set; }
    }
}