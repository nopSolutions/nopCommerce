using VerdeMart.OpenBoxesBridge.Messaging.Contracts;

namespace VerdeMart.OpenBoxesBridge.OpenBoxes;

public interface IOpenBoxesClient
{
    Task<CreateFulfillmentResult> CreateFulfillmentAsync(
        OrderPlacedMessage message,
        CancellationToken ct);
}
