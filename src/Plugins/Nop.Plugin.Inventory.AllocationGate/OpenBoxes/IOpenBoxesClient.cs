namespace Nop.Plugin.Inventory.AllocationGate.OpenBoxes;

public interface IOpenBoxesClient
{
    Task<IReadOnlyList<OpenBoxesFulfillmentOrder>> GetIssuedFulfillmentOrdersAsync(
        int batchSize, CancellationToken ct);
}
