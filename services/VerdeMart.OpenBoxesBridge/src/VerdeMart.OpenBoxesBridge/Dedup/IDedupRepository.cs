namespace VerdeMart.OpenBoxesBridge.Dedup;

public interface IDedupRepository
{
    Task EnsureSchemaAsync(CancellationToken ct);
    Task<bool> HasProcessedAsync(Guid orderGuid, CancellationToken ct);
    Task RecordProcessedAsync(Guid orderGuid, string? openBoxesFulfillmentId, CancellationToken ct);
}
