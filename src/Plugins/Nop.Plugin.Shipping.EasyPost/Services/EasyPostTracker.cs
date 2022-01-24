using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.EasyPost.Services
{
    /// <summary>
    /// Represents shipment tracker implementation
    /// </summary>
    public class EasyPostTracker : IShipmentTracker
    {
        #region Fields

        private readonly EasyPostService _easyPostService;
        private readonly IShippingPluginManager _shippingPluginManager;

        #endregion

        #region Ctor

        public EasyPostTracker(EasyPostService easyPostService,
            IShippingPluginManager shippingPluginManager)
        {
            _easyPostService = easyPostService;
            _shippingPluginManager = shippingPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get URL for a page to show tracking info (third party tracking page)
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the URL of a tracking page
        /// </returns>
        public async Task<string> GetUrlAsync(string trackingNumber, Shipment shipment = null)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                return string.Empty;

            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return string.Empty;

            return (await _easyPostService.GetTrackingUrlAsync(shipment, trackingNumber)).Url;
        }

        /// <summary>
        /// Get all shipment events
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of shipment events
        /// </returns>
        public async Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber, Shipment shipment = null)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                return new List<ShipmentStatusEvent>();

            if (!await _shippingPluginManager.IsPluginActiveAsync(EasyPostDefaults.SystemName))
                return new List<ShipmentStatusEvent>();

            return (await _easyPostService.GetTrackingEventsAsync(shipment, trackingNumber)).Events ?? new();
        }

        #endregion
    }
}