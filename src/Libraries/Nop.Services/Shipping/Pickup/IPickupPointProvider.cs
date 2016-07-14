using System.Web.Routing;
using Nop.Core.Domain.Common;
using Nop.Core.Plugins;
using Nop.Services.Shipping.Tracking;

namespace Nop.Services.Shipping.Pickup
{
    /// <summary>
    /// Represents an interface of pickup point provider
    /// </summary>
    public partial interface IPickupPointProvider : IPlugin
    {
        #region Properties

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        IShipmentTracker ShipmentTracker { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Get pickup points for the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Represents a response of getting pickup points</returns>
        GetPickupPointsResponse GetPickupPoints(Address address);

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues);

        #endregion
    }
}
