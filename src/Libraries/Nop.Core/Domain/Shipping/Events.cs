namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Shipment sent event
    /// </summary>
    public class ShipmentSentEvent
    {
        public ShipmentSentEvent(Shipment shipment)
        {
            this.Shipment = shipment;
        }

        /// <summary>
        /// Shipment
        /// </summary>
        public Shipment Shipment { get; private set; }
    }

    /// <summary>
    /// Shipment delivered event
    /// </summary>
    public class ShipmentDeliveredEvent
    {
        public ShipmentDeliveredEvent(Shipment shipment)
        {
            this.Shipment = shipment;
        }

        /// <summary>
        /// Shipment
        /// </summary>
        public Shipment Shipment { get; private set; }
    }
}