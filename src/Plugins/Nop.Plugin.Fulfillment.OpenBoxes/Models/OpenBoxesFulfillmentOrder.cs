namespace Nop.Plugin.Fulfillment.OpenBoxes.Models;

public record OpenBoxesFulfillmentOrder(
    string FulfillmentId,
    Guid OrderGuid,
    string Status,
    DateTime IssuedAtUtc);
