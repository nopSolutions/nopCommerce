namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Shipment delivered event
    /// </summary>
    public class ShipmentDeliveredEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public ShipmentDeliveredEvent(Shipment shipment)
        {
            this.Shipment = shipment;
        }

        /// <summary>
        /// Shipment
        /// </summary>
        public Shipment Shipment { get; }
    }
}