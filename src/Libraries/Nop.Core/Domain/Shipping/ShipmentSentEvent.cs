namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Shipment sent event
    /// </summary>
    public partial class ShipmentSentEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="shipment">Shipment</param>
        public ShipmentSentEvent(Shipment shipment)
        {
            Shipment = shipment;
        }

        /// <summary>
        /// Shipment
        /// </summary>
        public Shipment Shipment { get; }
    }
}