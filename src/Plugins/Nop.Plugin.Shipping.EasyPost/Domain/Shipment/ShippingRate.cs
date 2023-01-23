using System.Collections.Generic;

namespace Nop.Plugin.Shipping.EasyPost.Domain.Shipment
{
    /// <summary>
    /// Represents a shipping rate details
    /// </summary>
    public class ShippingRate
    {
        /// <summary>
        /// Gets or sets the rate id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the carrier name
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// Gets or sets the service level/name
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Gets or sets a shipping rate
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets a delivery days
        /// </summary>
        public int? DeliveryDays { get; set; }

        /// <summary>
        /// Gets or sets a currency code
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets a list of expected transit days at the specific percentiles
        /// </summary>
        public List<(int Percentile, int? DeliveryDays)> TimeInTransit { get; set; } = new();
    }
}