using System;

namespace Nop.Services.Shipping.Tracking
{
    /// <summary>
    /// Represents a shipment status event
    /// </summary>
    public partial class ShipmentStatusEvent
    {
        /// <summary>
        /// Gets or sets a status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets an event name
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets a location (address)
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets a two-letter country code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets a date
        /// </summary>
        public DateTime? Date { get; set; }
    }
}