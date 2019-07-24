//------------------------------------------------------------------------------
// Contributor(s): oskar.kjellin 
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Nop.Services.Shipping.Tracking
{
    /// <summary>
    /// Shipment tracker
    /// </summary>
    public partial interface IShipmentTracker
    {
        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>True if the tracker can track, otherwise false.</returns>
        bool IsMatch(string trackingNumber);

        /// <summary>
        /// Gets an URL for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>URL of a tracking page.</returns>
        string GetUrl(string trackingNumber);

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>List of Shipment Events.</returns>
        IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber);
    }
}
