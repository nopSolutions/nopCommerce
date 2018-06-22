using System;
using System.Linq;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping.Pickup;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Shipping extensions
    /// </summary>
    public static class ShippingExtensions
    {
        /// <summary>
        /// Is shipping rate computation method active
        /// </summary>
        /// <param name="srcm">Shipping rate computation method</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <returns>Result</returns>
        public static bool IsShippingRateComputationMethodActive(this IShippingRateComputationMethod srcm,
            ShippingSettings shippingSettings)
        {
            if (srcm == null)
                throw new ArgumentNullException(nameof(srcm));

            if (shippingSettings == null)
                throw new ArgumentNullException(nameof(shippingSettings));

            if (shippingSettings.ActiveShippingRateComputationMethodSystemNames == null)
                return false;
            foreach (var activeMethodSystemName in shippingSettings.ActiveShippingRateComputationMethodSystemNames)
                if (srcm.PluginDescriptor.SystemName.Equals(activeMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        /// <summary>
        /// Is pickup point provider active
        /// </summary>
        /// <param name="pickupPointProvider">Pickup point provider</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <returns>Result</returns>
        public static bool IsPickupPointProviderActive(this IPickupPointProvider pickupPointProvider, ShippingSettings shippingSettings)
        {
            if (pickupPointProvider == null)
                throw new ArgumentNullException(nameof(pickupPointProvider));

            if (shippingSettings == null)
                throw new ArgumentNullException(nameof(shippingSettings));

            if (shippingSettings.ActivePickupPointProviderSystemNames == null)
                return false;

            foreach (var activeProviderSystemName in shippingSettings.ActivePickupPointProviderSystemNames)
                if (pickupPointProvider.PluginDescriptor.SystemName.Equals(activeProviderSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;

        }

        /// <summary>
        /// Does country restriction exist
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Result</returns>
        public static bool CountryRestrictionExists(this ShippingMethod shippingMethod,
            int countryId)
        {
            if (shippingMethod == null)
                throw new ArgumentNullException(nameof(shippingMethod));

            var result = shippingMethod.ShippingMethodCountryMappings.Any(c => c.CountryId == countryId);
            return result;
        }
    }
}
