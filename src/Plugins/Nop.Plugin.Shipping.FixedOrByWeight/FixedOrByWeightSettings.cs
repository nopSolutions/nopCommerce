using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.FixedOrByWeight
{
    public class FixedOrByWeightSettings : ISettings
    {
        public bool LimitMethodsToCreated { get; set; }

        public bool ShippingByWeightEnabled { get; set; }
    }
}