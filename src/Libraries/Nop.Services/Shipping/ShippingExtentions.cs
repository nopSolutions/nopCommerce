
using System;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping
{
    public static class ShippingExtentions
    {
        public static bool IsShippingRateComputationMethodActive(this IShippingRateComputationMethod srcm,
            ShippingSettings shippingSettings)
        {
            if (srcm == null)
                throw new ArgumentNullException("srcm");

            if (shippingSettings == null)
                throw new ArgumentNullException("shippingSettings");

            if (shippingSettings.ActiveShippingRateComputationMethodSystemNames == null)
                return false;
            foreach (string activeMethodSystemName in shippingSettings.ActiveShippingRateComputationMethodSystemNames)
                if (srcm.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
    }
}
