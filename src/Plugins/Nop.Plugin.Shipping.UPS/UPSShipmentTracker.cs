using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nop.Plugin.Shipping.UPS.Services;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.UPS
{
    /// <summary>
    /// Represents the USP shipment tracker
    /// </summary>
    public class UPSShipmentTracker : IShipmentTracker
    {
        #region Fields

        private readonly UPSService _upsService;

        #endregion

        #region Ctor

        public UPSShipmentTracker(UPSService upsService)
        {
            _upsService = upsService;
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
            if (string.IsNullOrEmpty(trackingNumber))
                return false;

            //details on https://www.ups.com/us/en/tracking/help/tracking/tnh.page
            return Regex.IsMatch(trackingNumber, "^1Z[A-Z0-9]{16}$", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trackingNumber, "^\\d{9}$", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trackingNumber, "^T\\d{10}$", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trackingNumber, "^\\d{12}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Gets an URL for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>URL of a tracking page.</returns>
        public virtual string GetUrl(string trackingNumber)
        {
            return $"https://www.ups.com/track?&tracknum={trackingNumber}";
        }

        /// <summary>
        /// Gets all events for a tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track</param>
        /// <returns>List of Shipment Events.</returns>
        public virtual IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            var result = new List<ShipmentStatusEvent>();

            if (string.IsNullOrEmpty(trackingNumber))
                return result;

            result.AddRange(_upsService.GetShipmentEvents(trackingNumber));

            return result;
        }

        #endregion
    }
}