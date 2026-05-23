namespace Nop.Plugin.Inventory.AllocationGate.OpenBoxes;

public record OpenBoxesFulfillmentOrder(
    string FulfillmentId,
    Guid OrderGuid,
    string Status,
    DateTime IssuedAtUtc);
