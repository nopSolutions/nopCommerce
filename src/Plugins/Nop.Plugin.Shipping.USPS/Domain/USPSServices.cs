//------------------------------------------------------------------------------
// Contributor(s): RJH 08/07/2009, mb 10/20/2010. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Shipping.USPS.Domain
{
    /// <summary>
    /// Class for USPS V3 XML rate class -holds services offered by USPS
    /// </summary>
    public static class USPSServices
    {
        #region Fields

        /// <summary>
        /// USPS Domestic Services
        /// </summary>
        private static readonly Dictionary<string, string> _domesticServices = new Dictionary<string, string>
        {
            {"NONE (disable all domestic services)", "NONE"},
            {"First-Class", "0"},
            {"First-Class Mail Letter", "letter"},
            {"Priority Mail Express Sunday/Holiday Guarantee", "23"},
            {"Priority Mail Express Flat-Rate Envelope Sunday/Holiday Guarantee", "25"},
            {"Priority Mail Express Hold For Pickup", "2"},
            {"Priority Mail Express Flat Rate Envelope Hold For Pickup", "27"},
            {"Priority Mail Express", "3"},
            {"Priority Mail Express Flat Rate Envelope", "13"},
            {"Priority Mail", "1"},
            {"Priority Mail Flat Rate Envelope", "16"},
            {"Priority Mail Small Flat Rate Box", "28"},
            {"Priority Mail Medium Flat Rate Box", "17"},
            {"Priority Mail Large Flat Rate Box", "22"},
            {"Standard Post", "4"},
            {"Bound Printed Matter", "5"},
            {"Media Mail Parcel", "6"},
            {"Library Mail Parcel", "7"}
        };

        /// <summary>
        /// USPS International services
        /// </summary>
        private static readonly Dictionary<string, string> _internationalServices = new Dictionary<string, string>
        {
            {"NONE (disable all international services)", "NONE"},
            {"Global Express Guaranteed (GXG)", "4"},
            {"USPS GXG Envelopes", "12"},
            {"Priority Mail Express International Flat Rate Envelope", "10"},
            {"Priority Mail International", "2"},
            {"Priority Mail International Large Flat Rate Box", "11"},
            {"Priority Mail International Medium Flat Rate Box", "9"},
            {"Priority Mail International Small Flat Rate Box", "16"},
            {"First-Class Mail International Large Envelope", "14"},
            {"Priority Mail Express International", "1"},
            {"Priority Mail International Flat Rate Envelope", "8"},
            {"First-Class Package International Service", "15"}
        };

        #endregion

        #region Utilities

        /// <summary>
        /// Gets the Service ID for a domestic service
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns>service id or empty string if not found</returns>
        public static string GetServiceIdDomestic(string service)
        {
            var serviceId = "";
            if (String.IsNullOrEmpty(service))
                return serviceId;

            if (_domesticServices.ContainsKey(service))
                serviceId = _domesticServices[service];

            return serviceId;
        }

        /// <summary>
        /// Gets the Service ID for an International service
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns>service id or empty string</returns>
        public static string GetServiceIdInternational(string service)
        {
            var serviceId = "";
            if (String.IsNullOrEmpty(service))
                return serviceId;

            if (_internationalServices.ContainsKey(service))
                serviceId = _internationalServices[service];

            return serviceId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// USPS Domestic services string names
        /// </summary>
        public static string[] DomesticServices
        {
            get
            {
                return _domesticServices.Keys.Select(x => x).ToArray();
            }
        }

        /// <summary>
        /// USPS International services string names
        /// </summary>
        public static string[] InternationalServices
        {
            get
            {
                return _internationalServices.Keys.Select(x => x).ToArray();
            }
        }

        #endregion
    }
}