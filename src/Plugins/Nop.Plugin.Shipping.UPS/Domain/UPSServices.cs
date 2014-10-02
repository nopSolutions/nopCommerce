//------------------------------------------------------------------------------
// Contributor(s): mb 10/20/2010. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Shipping.UPS.Domain
{
    /// <summary>
    /// Class for UPS services
    /// </summary>
    public static class UPSServices
    {
        #region Fields

        /// <summary>
        /// UPS Service names
        /// </summary>
        private static readonly Dictionary<string, string> _services = new Dictionary<string, string>
        {
            {"UPS Next Day Air", "01"},
            {"UPS 2nd Day Air", "02"},
            {"UPS Ground", "03"},
            {"UPS Worldwide Express", "07"},
            {"UPS Worldwide Expedited", "08"},
            {"UPS Standard", "11"},
            {"UPS 3 Day Select", "12"},
            {"UPS Next Day Air Saver", "13"},
            {"UPS Next Day Air Early A.M.", "14"},
            {"UPS Worldwide Express Plus", "54"},
            {"UPS 2nd Day Air A.M.", "59"},
            {"UPS Saver", "65"},
            {"UPS Today Standard", "82"}, //82-86, for Polish Domestic Shipments
            {"UPS Today Dedicated Courier", "83"},
            {"UPS Today Express", "85"},
            {"UPS Today Express Saver", "86"}
        };

        #endregion
    
        #region Utilities

        /// <summary>
        /// Gets the Service ID for a service
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns>service id or empty string if not found</returns>
        public static string GetServiceId(string service)
        {
            var serviceId = "";
            if (String.IsNullOrEmpty(service))
                return serviceId;

            if (_services.ContainsKey(service))
                serviceId = _services[service];

            return serviceId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// UPS services string names
        /// </summary>
        public static string[] Services
        {
            get
            {
                return _services.Keys.Select(x => x).ToArray();
            }
        }

        #endregion

    }
}
