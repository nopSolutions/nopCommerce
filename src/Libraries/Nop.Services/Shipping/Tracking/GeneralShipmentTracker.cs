//------------------------------------------------------------------------------
// Contributor(s): oskar.kjellin 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="typeFinder">Type finder</param>
        public GeneralShipmentTracker(ITypeFinder typeFinder)
        {
            this._typeFinder = typeFinder;
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
                .Select(x => EngineContext.Current.ContainerManager.ResolveUnregistered(x) as IShipmentTracker)
                .ToList();
        }

        protected virtual IShipmentTracker GetTrackerByTrackingNumber(string trackingNumber)
        {
            return GetAllTrackers().FirstOrDefault(c => c.IsMatch(trackingNumber));
        }
        
        #endregion
        
        #region Methods

        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>True if the tracker can track, otherwise false.</returns>
        public virtual bool IsMatch(string trackingNumber)
        {
            var tracker = GetTrackerByTrackingNumber(trackingNumber);
            if (tracker != null)
                return tracker.IsMatch(trackingNumber);
            return false;
        }

        /// <summary>
        /// Gets a url for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>A url to a tracking page.</returns>
        public virtual string GetUrl(string trackingNumber)
        {
            var tracker = GetTrackerByTrackingNumber(trackingNumber);
            if (tracker != null)
                return tracker.GetUrl(trackingNumber);
            return null;
        }

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>List of Shipment Events.</returns>
        public virtual IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                return new List<ShipmentStatusEvent>();

            var tracker = GetTrackerByTrackingNumber(trackingNumber);
            if (tracker != null)
                return tracker.GetShipmentEvents(trackingNumber);
            return new List<ShipmentStatusEvent>();
        }

        #endregion
    }
}
