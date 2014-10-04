//------------------------------------------------------------------------------
// Contributor(s): oskar.kjellin 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using Nop.Plugin.Shipping.UPS.track;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.UPS
{
    public class UPSShipmentTracker : IShipmentTracker
    {
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly UPSSettings _upsSettings;

        public UPSShipmentTracker(ILogger logger, ILocalizationService localizationService, UPSSettings upsSettings)
        {
            this._logger = logger;
            this._localizationService = localizationService;
            this._upsSettings = upsSettings;
        }

        /// <summary>
        /// Gets if the current tracker can track the tracking number.
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>True if the tracker can track, otherwise false.</returns>
        public virtual bool IsMatch(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                return false;

            //Not sure if this is true for every shipment, but it is true for all of our shipments
            return trackingNumber.ToUpperInvariant().StartsWith("1Z");
        }

        /// <summary>
        /// Gets a url for a page to show tracking info (third party tracking page).
        /// </summary>
        /// <param name="trackingNumber">The tracking number to track.</param>
        /// <returns>A url to a tracking page.</returns>
        public virtual string GetUrl(string trackingNumber)
        {
            string url = "http://wwwapps.ups.com/WebTracking/track?trackNums={0}&track.x=Track";
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
            if (string.IsNullOrEmpty(trackingNumber))
                return new List<ShipmentStatusEvent>();

            var result = new List<ShipmentStatusEvent>();
            try
            {
                //use try-catch to ensure exception won't be thrown is web service is not available

                var track = new TrackService();
                var tr = new TrackRequest();
                var upss = new UPSSecurity();
                var upssSvcAccessToken = new UPSSecurityServiceAccessToken();
                upssSvcAccessToken.AccessLicenseNumber = _upsSettings.AccessKey;
                upss.ServiceAccessToken = upssSvcAccessToken;
                var upssUsrNameToken = new UPSSecurityUsernameToken();
                upssUsrNameToken.Username = _upsSettings.Username;
                upssUsrNameToken.Password = _upsSettings.Password;
                upss.UsernameToken = upssUsrNameToken;
                track.UPSSecurityValue = upss;
                var request = new RequestType();
                string[] requestOption = { "15" };
                request.RequestOption = requestOption;
                tr.Request = request;
                tr.InquiryNumber = trackingNumber;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                var trackResponse = track.ProcessTrack(tr);
                result.AddRange(trackResponse.Shipment.SelectMany(c => c.Package[0].Activity.Select(ToStatusEvent)).ToList());
            }
            catch (SoapException ex)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("SoapException Message= {0}.", ex.Message);
                sb.AppendFormat("SoapException Category:Code:Message= {0}.", ex.Detail.LastChild.InnerText);
                //sb.AppendFormat("SoapException XML String for all= {0}.", ex.Detail.LastChild.OuterXml);
                _logger.Error(string.Format("Error while getting UPS shipment tracking info - {0}", trackingNumber), new Exception(sb.ToString()));
            }
            catch (Exception exc)
            {
                _logger.Error(string.Format("Error while getting UPS shipment tracking info - {0}", trackingNumber), exc);
            }
            return result;
        }

        private ShipmentStatusEvent ToStatusEvent(ActivityType activity)
        {
            var ev = new ShipmentStatusEvent();
            switch (activity.Status.Type)
            {
                case "I":
                    if (activity.Status.Code == "DP")
                    {
                        ev.EventName = _localizationService.GetResource("Plugins.Shipping.UPS.Tracker.Departed");
                    }
                    else if (activity.Status.Code == "EP")
                    {
                        ev.EventName = _localizationService.GetResource("Plugins.Shipping.UPS.Tracker.ExportScanned");
                    }
                    else if (activity.Status.Code == "OR")
                    {
                        ev.EventName = _localizationService.GetResource("Plugins.Shipping.UPS.Tracker.OriginScanned");
                    }
                    else
                    {
                        ev.EventName = _localizationService.GetResource("Plugins.Shipping.UPS.Tracker.Arrived");
                    }
                    break;
                case "X":
                    ev.EventName = _localizationService.GetResource("Plugins.Shipping.UPS.Tracker.NotDelivered");
                    break;
                case "M":
                    ev.EventName = _localizationService.GetResource("Plugins.Shipping.UPS.Tracker.Booked");
                    break;
                case "D":
                    ev.EventName = _localizationService.GetResource("Plugins.Shipping.UPS.Tracker.Delivered");
                    break;
            }
            string dateString = string.Concat(activity.Date, " ", activity.Time);
            ev.Date = DateTime.ParseExact(dateString, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture);
            ev.CountryCode = activity.ActivityLocation.Address.CountryCode;
            ev.Location = activity.ActivityLocation.Address.City;
            return ev;
        }
    }

}