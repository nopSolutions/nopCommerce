using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if the tracker can track, otherwise false.
        /// </returns>
        public virtual Task<bool> IsMatchAsync(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                return Task.FromResult(false);

            //details on https://www.ups.com/us/en/tracking/help/tracking/tnh.page
            return Task.FromResult(Regex.IsMatch(trackingNumber, "^1Z[A-Z0-9]{16}$", RegexOptions.IgnoreCase) ||
                                   Regex.IsMatch(trackingNumber, "^\\d{9}$", RegexOptions.IgnoreCase) ||
                                   Regex.IsMatch(trackingNumber, "^T\\d{10}$", RegexOptions.IgnoreCase) ||
                                   Regex.IsMatch(trackingNumber, "^\\d{12}$", RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Gets an URL for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the uRL of a tracking page.
        /// </returns>
        public virtual Task<string> GetUrlAsync(string trackingNumber)
        {
            return Task.FromResult($"https://www.ups.com/track?&tracknum={trackingNumber}");
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
            var result = new List<ShipmentStatusEvent>();

            if (string.IsNullOrEmpty(trackingNumber))
                return result;

            result.AddRange(await _upsService.GetShipmentEventsAsync(trackingNumber));

            return result;
        }

        #endregion
    }
}