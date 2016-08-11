using System;
using System.Collections.Generic;
using Nop.Services.Logging;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.CanadaPost
{
    public class CanadaPostShipmentTracker : IShipmentTracker
    {
        #region Fields

        private readonly CanadaPostSettings _canadaPostSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public CanadaPostShipmentTracker(CanadaPostSettings canadaPostSettings,
            ILogger logger)
        {
            this._canadaPostSettings = canadaPostSettings;
            this._logger = logger;
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
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return false;

            //The PIN (Parcel Identification Number) assigned by Canada Post on creation of the shipping label and used for tracking purposes. (12, 13 or 16 characters)
            return trackingNumber.Length == 12 || trackingNumber.Length == 13 || trackingNumber.Length == 16;
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
            string errors;
            var trackingDetails = CanadaPostHelper.GetTrackingDetails(trackingNumber,
                _canadaPostSettings.ApiKey,_canadaPostSettings.UseSandbox, out errors);

            if (trackingDetails == null)
            {
                _logger.Error(errors);
                return new List<ShipmentStatusEvent>();
            }

            var events = new List<ShipmentStatusEvent>();
            foreach (var eventItem in trackingDetails.significantevents)
            {
                events.Add(new ShipmentStatusEvent
                {
                    EventName = eventItem.eventdescription,
                    Location = eventItem.eventsite,
                    Date = DateTime.Parse(eventItem.eventdate)
                });
            }

            return events;
        }

        #endregion
    }

}