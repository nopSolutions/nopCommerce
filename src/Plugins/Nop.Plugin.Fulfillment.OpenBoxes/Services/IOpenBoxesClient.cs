using Nop.Plugin.Fulfillment.OpenBoxes.Models;

namespace Nop.Plugin.Fulfillment.OpenBoxes.Services;

public interface IOpenBoxesClient
{
    Task<IReadOnlyList<OpenBoxesFulfillmentOrder>> GetIssuedFulfillmentOrdersAsync(
        int batchSize, CancellationToken ct);
}
