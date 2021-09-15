using System;
using System.Globalization;

namespace Nop.Plugin.Shipping.ShipStation
{
    public class ShipStationServiceRate
    {
        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Service code
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// Service code
        /// </summary>
        public string ShipmentCost { get; set; }

        /// <summary>
        /// Other cost
        /// </summary>
        public string OtherCost { get; set; }

        /// <summary>
        /// Total cost
        /// </summary>
        public decimal TotalCost => Convert.ToDecimal(ShipmentCost, new CultureInfo("en-US")) + Convert.ToDecimal(OtherCost, new CultureInfo("en-US"));
    }
}