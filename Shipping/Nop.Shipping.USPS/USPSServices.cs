//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): RJH 08/07/2009, mb 10/20/2010. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Shipping.Methods.USPS
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
                                                "First-Class",
                                                "Express Mail Sunday/Holiday Guarantee",
                                                "Express Mail Flat-Rate Envelope Sunday/Holiday Guarantee",
                                                "Express Mail Hold For Pickup",
                                                "Express Mail Flat Rate Envelope Hold For Pickup",
                                                "Express Mail",
                                                "Express Mail Flat Rate Envelope",
                                                "Priority Mail",
                                                "Priority Mail Flat Rate Envelope",
                                                "Priority Mail Small Flat Rate Box",
                                                "Priority Mail Medium Flat Rate Box",
                                                "Priority Mail Large Flat Rate Box",
                                                "Parcel Post",
                                                "Bound Printed Matter",
                                                "Media Mail",
                                                "Library Mail"
                                                };

        /// <summary>
        /// V3 USPS International services
        /// </summary>
        private string[] _internationalServices = {                                    
                                                    "Global Express Guaranteed (GXG)",
                                                    "Global Express Guaranteed Non-Document Rectangular",
                                                    "Global Express Guaranteed Non-Document Non-Rectangular",
                                                    "USPS GXG Envelopes",
                                                    "Express Mail International Flat Rate Envelope",
                                                    "Priority Mail International",
                                                    "Priority Mail International Large Flat Rate Box",
                                                    "Priority Mail International Medium Flat Rate Box",
                                                    "Priority Mail International Small Flat Rate Box",
                                                    "First-Class Mail International Large Envelope",
                                                    "Express Mail International",
                                                    "Priority Mail International Flat Rate Envelope",
                                                    "First-Class Mail International Package"
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
                case "First-Class":
                    serviceId = "0";
                    break;
                case "Express Mail Sunday/Holiday Guarantee":
                    serviceId = "23";
                    break;
                case "Express Mail Flat-Rate Envelope Sunday/Holiday Guarantee":
                    serviceId = "25";
                    break;
                case "Express Mail Hold For Pickup":
                    serviceId = "2";
                    break;
                case "Express Mail Flat Rate Envelope Hold For Pickup":
                    serviceId = "27";
                    break;
                case "Express Mail":
                    serviceId = "3";
                    break;
                case "Express Mail Flat Rate Envelope":
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
                case "Parcel Post":
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
                case "Express Mail International Flat Rate Envelope":
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
                case "Express Mail International":
                    serviceId = "1";
                    break;
                case "Priority Mail International Flat Rate Envelope":
                    serviceId = "8";
                    break;
                case "First-Class Mail International Package":
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