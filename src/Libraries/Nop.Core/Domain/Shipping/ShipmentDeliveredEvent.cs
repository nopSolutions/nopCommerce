namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Shipment delivered event
/// </summary>
public partial class ShipmentDeliveredEvent
{
    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="shipment">Shipment</param>
    public ShipmentDeliveredEvent(Shipment shipment)
    {
        Shipment = shipment;
    }

    /// <summary>
    /// Shipment
    /// </summary>
    public Shipment Shipment { get; }
}