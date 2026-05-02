using Dapper;
using Microsoft.Data.Sqlite;

namespace OutboxSpike;

// Uses a shared named in-memory database. The _anchor connection is kept open for
// the lifetime of the repository so the in-memory DB survives across multiple
// short-lived Dapper connections (SQLite in-memory databases are destroyed when
// the last connection closes).
public sealed class OutboxRepository : IDisposable
{
    private readonly string _connectionString;
    private readonly SqliteConnection _anchor;

    public OutboxRepository(string connectionString)
    {
        _connectionString = connectionString;
        _anchor = new SqliteConnection(_connectionString);
        _anchor.Open();
    }

    public void InitSchema()
    {
        _anchor.Execute("""
            CREATE TABLE IF NOT EXISTS OutboxMessage (
                Id        TEXT PRIMARY KEY,
                Payload   TEXT NOT NULL,
                Status    TEXT NOT NULL DEFAULT 'Pending',
                CreatedAt TEXT NOT NULL,
                SentAt    TEXT
            );
            """);
    }

    public async Task InsertAsync(string id, string payload)
    {
        await using var conn = await OpenAsync();
        await conn.ExecuteAsync(
            "INSERT INTO OutboxMessage (Id, Payload, Status, CreatedAt) VALUES (@Id, @Payload, 'Pending', @CreatedAt)",
            new { Id = id, Payload = payload, CreatedAt = DateTime.UtcNow.ToString("O") });
    }

    public async Task<IEnumerable<OutboxMessage>> GetPendingAsync(int batchSize = 10)
    {
        await using var conn = await OpenAsync();
        return await conn.QueryAsync<OutboxMessage>(
            "SELECT Id, Payload, Status, CreatedAt, SentAt FROM OutboxMessage WHERE Status = 'Pending' LIMIT @Limit",
            new { Limit = batchSize });
    }

    public async Task MarkSentAsync(string id)
    {
        await using var conn = await OpenAsync();
        await conn.ExecuteAsync(
            "UPDATE OutboxMessage SET Status = 'Sent', SentAt = @SentAt WHERE Id = @Id",
            new { Id = id, SentAt = DateTime.UtcNow.ToString("O") });
    }

    public async Task<int> CountByStatusAsync(string status)
    {
        await using var conn = await OpenAsync();
        return await conn.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM OutboxMessage WHERE Status = @Status",
            new { Status = status });
    }

    private async Task<SqliteConnection> OpenAsync()
    {
        var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }

    public void Dispose() => _anchor.Dispose();
}

public record OutboxMessage(string Id, string Payload, string Status, string CreatedAt, string? SentAt);
