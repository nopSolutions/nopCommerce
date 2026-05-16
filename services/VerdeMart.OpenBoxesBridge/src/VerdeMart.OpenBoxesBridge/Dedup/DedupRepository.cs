using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace VerdeMart.OpenBoxesBridge.Dedup;

public class DedupRepository : IDedupRepository
{
    private readonly string _connectionString;

    public DedupRepository(IOptions<BridgeSettings> options)
    {
        _connectionString = options.Value.DedupConnectionString;
    }

    public async Task EnsureSchemaAsync(CancellationToken ct)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS processed_orders (
                order_guid TEXT PRIMARY KEY,
                processed_at_utc TEXT NOT NULL,
                openboxes_fulfillment_id TEXT NULL
            );
            """;
        await command.ExecuteNonQueryAsync(ct);
    }

    public async Task<bool> HasProcessedAsync(Guid orderGuid, CancellationToken ct)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM processed_orders WHERE order_guid = $id LIMIT 1;";
        command.Parameters.AddWithValue("$id", orderGuid.ToString());

        var result = await command.ExecuteScalarAsync(ct);
        return result is not null;
    }

    public async Task RecordProcessedAsync(Guid orderGuid, string? openBoxesFulfillmentId, CancellationToken ct)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT OR IGNORE INTO processed_orders (order_guid, processed_at_utc, openboxes_fulfillment_id)
            VALUES ($id, $ts, $fid);
            """;
        command.Parameters.AddWithValue("$id", orderGuid.ToString());
        command.Parameters.AddWithValue("$ts", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("$fid", (object?)openBoxesFulfillmentId ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(ct);
    }
}
