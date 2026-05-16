namespace VerdeMart.OpenBoxesBridge.OpenBoxes;

public abstract record CreateFulfillmentResult
{
    public sealed record Success(string FulfillmentId) : CreateFulfillmentResult;
    public sealed record Duplicate(string FulfillmentId) : CreateFulfillmentResult;
    public sealed record Failure(string Reason, bool Transient) : CreateFulfillmentResult;
}
