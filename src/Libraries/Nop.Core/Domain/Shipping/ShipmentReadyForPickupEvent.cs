namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Shipment ready for pickup event
    /// </summary>
    public class ShipmentReadyForPickupEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public ShipmentReadyForPickupEvent(Shipment shipment)
        {
            Shipment = shipment;
        }

        /// <summary>
        /// Shipment
        /// </summary>
        public Shipment Shipment { get; }
    }
}
