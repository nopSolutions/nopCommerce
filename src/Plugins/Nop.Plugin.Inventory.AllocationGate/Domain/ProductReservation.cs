using Nop.Core;

namespace Nop.Plugin.Inventory.AllocationGate.Domain;

public class ProductReservation : BaseEntity
{
    public string ReservationKey { get; set; }
    public string ChannelKey { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public int Status { get; set; }
    public DateTime? ReservedUntilUtc { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}

public enum ReservationStatus
{
    Active = 0,
    Committed = 1,
    Released = 2,
    Expired = 3
}
