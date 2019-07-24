//------------------------------------------------------------------------------
// Contributor(s): oskar.kjellin 
//------------------------------------------------------------------------------

using System;

namespace Nop.Services.Shipping.Tracking
{
    /// <summary>
    /// Shipment status event
    /// </summary>
    public partial class ShipmentStatusEvent
    {
        /// <summary>
        /// Event name
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// two-letter country code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime? Date { get; set; }
    }
}
