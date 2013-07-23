//------------------------------------------------------------------------------
// Contributor(s): RJH 08/07/2009, mb 10/20/2010. 
//------------------------------------------------------------------------------

namespace Nop.Plugin.Shipping.USPS.Domain
{
    /// <summary>
    /// Class for USPS V3 XML rate class -holds services offered by USPS
    /// </summary>
    public class USPSServices
    {
        /// <summary>
        /// V3 USPS Domestic Services
        /// </summary>
        private string[] _domesticServices = {
                                                "NONE (disable all domestic services)",
                                                "First-Class",
                                                "Priority Mail Express Sunday/Holiday Guarantee",
                                                "Priority Mail Express Flat-Rate Envelope Sunday/Holiday Guarantee",
                                                "Priority Mail Express Hold For Pickup",
                                                "Priority Mail Express Flat Rate Envelope Hold For Pickup",
                                                "Priority Mail Express",
                                                "Priority Mail Express Flat Rate Envelope",
                                                "Priority Mail",
                                                "Priority Mail Flat Rate Envelope",
                                                "Priority Mail Small Flat Rate Box",
                                                "Priority Mail Medium Flat Rate Box",
                                                "Priority Mail Large Flat Rate Box",
                                                "Standard Post",
                                                "Bound Printed Matter",
                                                "Media Mail",
                                                "Library Mail"
                                                };

        /// <summary>
        /// V3 USPS International services
        /// </summary>
        private string[] _internationalServices = {    
                                                    "NONE (disable all international services)",                                
                                                    "Global Express Guaranteed (GXG)",
                                                    "Global Express Guaranteed Non-Document Rectangular",
                                                    "Global Express Guaranteed Non-Document Non-Rectangular",
                                                    "USPS GXG Envelopes",
                                                    "Priority Mail Express International Flat Rate Envelope",
                                                    "Priority Mail International",
                                                    "Priority Mail International Large Flat Rate Box",
                                                    "Priority Mail International Medium Flat Rate Box",
                                                    "Priority Mail International Small Flat Rate Box",
                                                    "First-Class Mail International Large Envelope",
                                                    "Priority Mail Express International",
                                                    "Priority Mail International Flat Rate Envelope",
                                                    "First-Class Package International Service"
                                                    };

        #region Properties

        /// <summary>
        /// USPS Domestic services string names
        /// </summary>
        public string[] DomesticServices
        {
            get { return _domesticServices; }
        }

        /// <summary>
        /// USPS International services string names
        /// </summary>
        public string[] InternationalServices
        {
            get { return _internationalServices; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets the Service ID for a domestic service
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns>service id or empty string if not found</returns>
        public static string GetServiceIdDomestic(string service)
        {
            string serviceId = string.Empty;
            switch (service)
            {
                case "NONE (disable all domestic services)":
                    serviceId = "NONE";
                    break;
                case "First-Class":
                    serviceId = "0";
                    break;
                case "Priority Mail Express Sunday/Holiday Guarantee":
                    serviceId = "23";
                    break;
                case "Priority Mail Express Flat-Rate Envelope Sunday/Holiday Guarantee":
                    serviceId = "25";
                    break;
                case "Priority Mail Express Hold For Pickup":
                    serviceId = "2";
                    break;
                case "Priority Mail Express Flat Rate Envelope Hold For Pickup":
                    serviceId = "27";
                    break;
                case "Priority Mail Express":
                    serviceId = "3";
                    break;
                case "Priority Mail Express Flat Rate Envelope":
                    serviceId = "13";
                    break;
                case "Priority Mail":
                    serviceId = "1";
                    break;
                case "Priority Mail Flat Rate Envelope":
                    serviceId = "16";
                    break;
                case "Priority Mail Small Flat Rate Box":
                    serviceId = "28";
                    break;
                case "Priority Mail Medium Flat Rate Box":
                    serviceId = "17";
                    break;
                case "Priority Mail Large Flat Rate Box":
                    serviceId = "22";
                    break;
                case "Standard Post":
                    serviceId = "4";
                    break;
                case "Bound Printed Matter":
                    serviceId = "5";
                    break;
                case "Media Mail":
                    serviceId = "6";
                    break;
                case "Library Mail":
                    serviceId = "7";
                    break;
                default:
                    break;
            }
            return serviceId;
        }

        /// <summary>
        /// Gets the Service ID for an International service
        /// </summary>
        /// <param name="service">service name</param>
        /// <returns>service id or emtpy string</returns>
        public static string GetServiceIdInternational(string service)
        {
            string serviceId = string.Empty;
            switch (service)
            {
                case "NONE (disable all international services)":
                    serviceId = "NONE";
                    break;
                case "Global Express Guaranteed (GXG)":
                    serviceId = "4";
                    break;
                case "Global Express Guaranteed Non-Document Rectangular":
                    serviceId = "6";
                    break;
                case "Global Express Guaranteed Non-Document Non-Rectangular":
                    serviceId = "7";
                    break;
                case "USPS GXG Envelopes":
                    serviceId = "12";
                    break;
                case "Priority Mail Express International Flat Rate Envelope":
                    serviceId = "10";
                    break;
                case "Priority Mail International":
                    serviceId = "2";
                    break;
                case "Priority Mail International Large Flat Rate Box":
                    serviceId = "11";
                    break;
                case "Priority Mail International Medium Flat Rate Box":
                    serviceId = "9";
                    break;
                case "Priority Mail International Small Flat Rate Box":
                    serviceId = "16";
                    break;
                case "First-Class Mail International Large Envelope":
                    serviceId = "14";
                    break;
                case "Priority Mail Express International":
                    serviceId = "1";
                    break;
                case "Priority Mail International Flat Rate Envelope":
                    serviceId = "8";
                    break;
                case "First-Class Package International Service":
                    serviceId = "15";
                    break;
                default:
                    break;
            }
            return serviceId;
        }

        #endregion
    }
}