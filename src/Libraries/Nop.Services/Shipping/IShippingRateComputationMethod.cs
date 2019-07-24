using Nop.Services.Plugins;
using Nop.Services.Shipping.Tracking;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Provides an interface of shipping rate computation method
    /// </summary>
    public partial interface IShippingRateComputationMethod : IPlugin
    {
        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        ShippingRateComputationMethodType ShippingRateComputationMethodType { get; }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest);

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest);

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        IShipmentTracker ShipmentTracker { get; }
    }
}