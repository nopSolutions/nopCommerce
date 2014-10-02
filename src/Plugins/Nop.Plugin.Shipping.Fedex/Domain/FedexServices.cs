//------------------------------------------------------------------------------
// Contributor(s): mb. 
//------------------------------------------------------------------------------

namespace Nop.Plugin.Shipping.Fedex.Domain
{
    /// <summary>
    /// Class for FedEx services
    /// </summary>
    public class FedexServices
    {
        /// <summary>
        /// FedEx Service names
        /// </summary>
        private readonly string[] _services = {
                                        "FedEx Europe First International Priority",
                                        "FedEx 1Day Freight",
                                        "FedEx 2Day",
                                        "FedEx 2Day Freight",
                                        "FedEx 3Day Freight",
                                        "FedEx Express Saver",
                                        "FedEx Ground",
                                        "FedEx First Overnight",
                                        "FedEx Ground Home Delivery",
                                        "FedEx International Distribution Freight",
                                        "FedEx International Economy",
                                        "FedEx International Economy Distribution",
                                        "FedEx International Economy Freight",
                                        "FedEx International First",
                                        "FedEx International Priority",
                                        "FedEx International Priority Freight",
                                        "FedEx Priority Overnight",
                                        "FedEx Smart Post",
                                        "FedEx Standard Overnight",
                                        "FedEx Freight",
                                        "FedEx National Freight"
                                        };

        #region Properties

        /// <summary>
        /// FedEx services string names
        /// </summary>
        public string[] Services
        {
            get { return _services; }
        }

        #endregion

        #region Utilities
        /// <summary>
        /// Gets the text name based on the ServiceID (in FedEx Reply)
        /// </summary>
        /// <param name="serviceId">ID of the carrier service -from FedEx</param>
        /// <returns>String representation of the carrier service</returns>
        public static string GetServiceName(string serviceId)
        {
            switch (serviceId)
            {
                case "EUROPE_FIRST_INTERNATIONAL_PRIORITY":
                    return "FedEx Europe First International Priority";
                case "FEDEX_1_DAY_FREIGHT":
                    return "FedEx 1Day Freight";
                case "FEDEX_2_DAY":
                    return "FedEx 2Day";
                case "FEDEX_2_DAY_FREIGHT":
                    return "FedEx 2Day Freight";
                case "FEDEX_3_DAY_FREIGHT":
                    return "FedEx 3Day Freight";
                case "FEDEX_EXPRESS_SAVER":
                    return "FedEx Express Saver";
                case "FEDEX_GROUND":
                    return "FedEx Ground";
                case "FIRST_OVERNIGHT":
                    return "FedEx First Overnight";
                case "GROUND_HOME_DELIVERY":
                    return "FedEx Ground Home Delivery";
                case "INTERNATIONAL_DISTRIBUTION_FREIGHT":
                    return "FedEx International Distribution Freight";
                case "INTERNATIONAL_ECONOMY":
                    return "FedEx International Economy";
                case "INTERNATIONAL_ECONOMY_DISTRIBUTION":
                    return "FedEx International Economy Distribution";
                case "INTERNATIONAL_ECONOMY_FREIGHT":
                    return "FedEx International Economy Freight";
                case "INTERNATIONAL_FIRST":
                    return "FedEx International First";
                case "INTERNATIONAL_PRIORITY":
                    return "FedEx International Priority";
                case "INTERNATIONAL_PRIORITY_FREIGHT":
                    return "FedEx International Priority Freight";
                case "PRIORITY_OVERNIGHT":
                    return "FedEx Priority Overnight";
                case "SMART_POST":
                    return "FedEx Smart Post";
                case "STANDARD_OVERNIGHT":
                    return "FedEx Standard Overnight";
                case "FEDEX_FREIGHT":
                    return "FedEx Freight";
                case "FEDEX_NATIONAL_FREIGHT":
                    return "FedEx National Freight";
                default:
                    return "UNKNOWN";
            }
        }

        /// <summary>
        /// Gets the ServiceId based on the text name
        /// </summary>
        /// <param name="serviceName">Name of the carrier service (based on the text name returned from GetServiceName())</param>
        /// <returns>Service ID as used by FedEx</returns>
        public static string GetServiceId(string serviceName)
        {
            switch (serviceName)
            {
                case "FedEx Europe First International Priority":
                    return "EUROPE_FIRST_INTERNATIONAL_PRIORITY";
                case "FedEx 1Day Freight":
                    return "FEDEX_1_DAY_FREIGHT";
                case "FedEx 2Day":
                    return "FEDEX_2_DAY";
                case "FedEx 2Day Freight":
                    return "FEDEX_2_DAY_FREIGHT";
                case "FedEx 3Day Freight":
                    return "FEDEX_3_DAY_FREIGHT";
                case "FedEx Express Saver":
                    return "FEDEX_EXPRESS_SAVER";
                case "FedEx Ground":
                    return "FEDEX_GROUND";
                case "FedEx First Overnight":
                    return "FIRST_OVERNIGHT";
                case "FedEx Ground Home Delivery":
                    return "GROUND_HOME_DELIVERY";
                case "FedEx International Distribution Freight":
                    return "INTERNATIONAL_DISTRIBUTION_FREIGHT";
                case "FedEx International Economy":
                    return "INTERNATIONAL_ECONOMY";
                case "FedEx International Economy Distribution":
                    return "INTERNATIONAL_ECONOMY_DISTRIBUTION";
                case "FedEx International Economy Freight":
                    return "INTERNATIONAL_ECONOMY_FREIGHT";
                case "FedEx International First":
                    return "INTERNATIONAL_FIRST";
                case "FedEx International Priority":
                    return "INTERNATIONAL_PRIORITY";
                case "FedEx International Priority Freight":
                    return "INTERNATIONAL_PRIORITY_FREIGHT";
                case "FedEx Priority Overnight":
                    return "PRIORITY_OVERNIGHT";
                case "FedEx Smart Post":
                    return "SMART_POST";
                case "FedEx Standard Overnight":
                    return "STANDARD_OVERNIGHT";
                case "FedEx Freight":
                    return "FEDEX_FREIGHT";
                case "FedEx National Freight":
                    return "FEDEX_NATIONAL_FREIGHT";
                default:
                    return "UNKNOWN";
            }
        }
        #endregion

    }
}
