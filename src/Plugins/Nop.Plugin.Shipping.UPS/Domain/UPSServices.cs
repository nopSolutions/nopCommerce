//------------------------------------------------------------------------------
// Contributor(s): mb 10/20/2010. 
//------------------------------------------------------------------------------

namespace Nop.Plugin.Shipping.UPS.Domain
{
    /// <summary>
    /// Class for UPS services
    /// </summary>
    public class UPSServices
    {
        /// <summary>
        /// UPS Service names
        /// </summary>
        private string[] _services = {
                                        "UPS Next Day Air",
                                        "UPS 2nd Day Air",
                                        "UPS Ground",
                                        "UPS Worldwide Express",
                                        "UPS Worldwide Expedited",
                                        "UPS Standard",
                                        "UPS 3 Day Select",
                                        "UPS Next Day Air Saver",
                                        "UPS Next Day Air Early A.M.",
                                        "UPS Worldwide Express Plus",
                                        "UPS 2nd Day Air A.M.",
                                        "UPS Saver", 
                                        "UPS Today Standard",
                                        "UPS Today Dedicated Courrier",
                                        "UPS Today Express",
                                        "UPS Today Express Saver"
                                        };

        #region Properties

        /// <summary>
        /// UPS services string names
        /// </summary>
        public string[] Services
        {
            get { return _services; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets the Service ID for a service
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns>service id or empty string if not found</returns>
        public static string GetServiceId(string service)
        {
            string serviceId = string.Empty;
            switch (service)
            {
                case "UPS Next Day Air":
                    serviceId = "01";
                    break;
                case "UPS 2nd Day Air":
                    serviceId = "02";
                    break;
                case "UPS Ground":
                    serviceId = "03";
                    break;
                case "UPS Worldwide Express":
                    serviceId = "07";
                    break;
                case "UPS Worldwide Expedited":
                    serviceId = "08";
                    break;
                case "UPS Standard":
                    serviceId = "11";
                    break;
                case "UPS 3 Day Select":
                    serviceId = "12";
                    break;
                case "UPS Next Day Air Saver":
                    serviceId = "13";
                    break;
                case "UPS Next Day Air Early A.M.":
                    serviceId = "14";
                    break;
                case "UPS Worldwide Express Plus":
                    serviceId = "54";
                    break;
                case "UPS 2nd Day Air A.M.":
                    serviceId = "59";
                    break;
                case "UPS Saver":
                    serviceId = "65";
                    break;
                case "UPS Today Standard": //82-86, for Polish Domestic Shipments
                    serviceId = "82";
                    break;
                case "UPS Today Dedicated Courier":
                    serviceId = "83";
                    break;
                case "UPS Today Express":
                    serviceId = "85";
                    break;
                case "UPS Today Express Saver":
                    serviceId = "86";
                    break;
                default:
                    break;
            }
            return serviceId;
        }

        #endregion
    }
}
