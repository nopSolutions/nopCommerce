namespace Nop.Plugin.Inventory.AllocationGate.Models;

public class ReserveRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public string ReservationKey { get; set; }
    public int TtlSeconds { get; set; } = 300;
}
