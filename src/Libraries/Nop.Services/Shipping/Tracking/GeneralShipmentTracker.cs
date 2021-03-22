//------------------------------------------------------------------------------
// Contributor(s): oskar.kjellin 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;

namespace Nop.Services.Shipping.Tracking
{
    /// <summary>
    /// General shipment tracker (finds an appropriate tracker by tracking number)
    /// </summary>
    public partial class GeneralShipmentTracker : IShipmentTracker
    {
        #region Fields

        private readonly ITypeFinder _typeFinder;

        #endregion

        #region Ctor

        public GeneralShipmentTracker(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all trackers
        /// </summary>
        /// <returns>All available shipment trackers</returns>
        protected virtual IList<IShipmentTracker> GetAllTrackers()
        {
            return _typeFinder.FindClassesOfType<IShipmentTracker>()
                //exclude this one
                .Where(x => x != typeof(GeneralShipmentTracker))
                .Select(x => EngineContext.Current.ResolveUnregistered(x) as IShipmentTracker)
                .ToList();
        }

        /// <summary>
        /// Get tracker by tracking number
        /// </summary>
        /// <param name="trackingNumber">Tracking number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the racker (IShipmentTracker)
        /// </returns>
        protected virtual async Task<IShipmentTracker> GetTrackerByTrackingNumberAsync(string trackingNumber)
        {
            return await GetAllTrackers().FirstOrDefaultAwaitAsync(async c => await c.IsMatchAsync(trackingNumber));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if the tracker can track, otherwise false.
        /// </returns>
        public virtual async Task<bool> IsMatchAsync(string trackingNumber)
        {
            var tracker = await GetTrackerByTrackingNumberAsync(trackingNumber);
            if (tracker != null)
                return await tracker.IsMatchAsync(trackingNumber);
            return false;
        }

        /// <summary>
        /// Gets an URL for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the uRL of a tracking page.
        /// </returns>
        public virtual async Task<string> GetUrlAsync(string trackingNumber)
        {
            var tracker = await GetTrackerByTrackingNumberAsync(trackingNumber);

            if (tracker == null)
                return null;

            return await tracker.GetUrlAsync(trackingNumber);
        }

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of Shipment Events.
        /// </returns>
        public virtual async Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                return new List<ShipmentStatusEvent>();

            var tracker = await GetTrackerByTrackingNumberAsync(trackingNumber);
            if (tracker != null)
                return await tracker.GetShipmentEventsAsync(trackingNumber);
            return new List<ShipmentStatusEvent>();
        }

        #endregion
    }
}