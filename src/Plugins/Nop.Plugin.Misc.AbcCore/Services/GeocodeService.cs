using System.Net;
using System.Xml.Linq;
using Nop.Core;
using SevenSpikes.Nop.Plugins.StoreLocator.Domain;

namespace Nop.Plugin.Misc.AbcCore.Services
{
    public class GeocodeService : IGeocodeService
    {
        private readonly string ApiKey;

        public GeocodeService(
            CoreSettings coreSettings)
        {
            ApiKey = coreSettings.GoogleMapsGeocodingAPIKey;
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                throw new NopException("No Google Maps API key provided, please provide an API key in configuration to enable geocoding.");
            }
        }

        public (double lat, double lng) GeocodeZip(string zip) {
            string requestUri = 
                string.Format(
                    "https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false",
                    zip, ApiKey);

            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();
            XDocument xdoc = XDocument.Load(response.GetResponseStream());

            XElement geocodeResponse = xdoc.Element("GeocodeResponse");
            XElement status = geocodeResponse.Element("status");

            if (status.Value.Equals("REQUEST_DENIED"))
            {
                var errorMessage = geocodeResponse.Element("error_message").Value;

                throw new NopException($"Error returned while geocoding: {errorMessage}");
            }

            if (status.Value.Equals("ZERO_RESULTS"))
            {
                return new (0, 0);
            }

            XElement result = geocodeResponse.Element("result");
            XElement locationElement = result.Element("geometry").Element("location");
            XElement lat = locationElement.Element("lat");
            XElement lng = locationElement.Element("lng");

            return new (float.Parse(lat.Value), float.Parse(lng.Value));
        }
    }
}