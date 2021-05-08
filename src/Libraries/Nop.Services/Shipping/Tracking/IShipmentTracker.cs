//------------------------------------------------------------------------------
// Contributor(s): oskar.kjellin 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if the tracker can track, otherwise false.
        /// </returns>
        Task<bool> IsMatchAsync(string trackingNumber);

        /// <summary>
        /// Gets an URL for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the uRL of a tracking page.
        /// </returns>
        Task<string> GetUrlAsync(string trackingNumber);

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of Shipment Events.
        /// </returns>
        Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber);
    }
}
