using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping.Tracking
{
    /// <summary>
    /// Represents a shipment tracker
    /// </summary>
    public partial interface IShipmentTracker
    {
        /// <summary>
        /// Get URL for a page to show tracking info (third party tracking page)
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the URL of a tracking page
        /// </returns>
        Task<string> GetUrlAsync(string trackingNumber, Shipment shipment = null);

        /// <summary>
        /// Get all shipment events
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <param name="shipment">Shipment; pass null if the tracking number is not associated with a specific shipment</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of shipment events
        /// </returns>
        Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber, Shipment shipment = null);
    }
}