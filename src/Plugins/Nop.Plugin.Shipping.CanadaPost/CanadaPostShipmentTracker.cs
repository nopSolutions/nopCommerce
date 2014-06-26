using System.Collections.Generic;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.CanadaPost
{
    public class CanadaPostShipmentTracker : IShipmentTracker
    {
        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>True if the tracker can track, otherwise false.</returns>
        public virtual bool IsMatch(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return false;

            //What is a Canada Post tracking number format?
            return false;
        }

        /// <summary>
        /// Gets a url for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>A url to a tracking page.</returns>
        public virtual string GetUrl(string trackingNumber)
        {
            string url = "http://www.canadapost.ca/cpotools/apps/track/personal/findByTrackNumber?trackingNumber={0}&LOCALE=en";
            url = string.Format(url, trackingNumber);
            return url;
        }

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>List of Shipment Events.</returns>
        public virtual IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            return new List<ShipmentStatusEvent>();
        }
    }

}