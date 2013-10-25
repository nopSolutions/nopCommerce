namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Shipment sent event
    /// </summary>
    public class ShipmentSentEvent
    {
        private readonly Shipment _shipment;

        public ShipmentSentEvent(Shipment shipment)
        {
            this._shipment = shipment;
        }

        public Shipment Shipment
        {
            get { return _shipment; }
        }
    }

    /// <summary>
    /// Shipment delivered event
    /// </summary>
    public class ShipmentDeliveredEvent
    {
        private readonly Shipment _shipment;

        public ShipmentDeliveredEvent(Shipment shipment)
        {
            this._shipment = shipment;
        }

        public Shipment Shipment
        {
            get { return _shipment; }
        }
    }
}