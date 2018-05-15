using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping.Tracking
{
    /// <summary>
    /// Shipment extensions
    /// </summary>
    public static class ShipmentExtensions
    {
        /// <summary>
        /// Get the tracker of the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="shippingService">Shipping service</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <returns>Shipment tracker</returns>
        public static IShipmentTracker GetShipmentTracker(this Shipment shipment, IShippingService shippingService, ShippingSettings shippingSettings)
        {
            if (!shipment.Order.PickUpInStore)
            {
                var shippingRateComputationMethod = shippingService.LoadShippingRateComputationMethodBySystemName(shipment.Order.ShippingRateComputationMethodSystemName);
                if (shippingRateComputationMethod != null && 
                    shippingRateComputationMethod.PluginDescriptor.Installed)
                    //shippingRateComputationMethod.IsShippingRateComputationMethodActive(shippingSettings))
                    return shippingRateComputationMethod.ShipmentTracker;
            }
            else
            {
                var pickupPointProvider = shippingService.LoadPickupPointProviderBySystemName(shipment.Order.ShippingRateComputationMethodSystemName);
                if (pickupPointProvider != null && 
                    pickupPointProvider.PluginDescriptor.Installed)
                    //pickupPointProvider.IsPickupPointProviderActive(shippingSettings))
                    return pickupPointProvider.ShipmentTracker;
            }

            return null;
        }
    }
}