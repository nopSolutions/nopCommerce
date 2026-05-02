using OutboxSpike;

// --live mode: orders arrive every 3s, dispatcher runs, you control RabbitMQ manually.
// Usage:  dotnet run -- --live
if (args.Contains("--live"))
{
    await RunLiveModeAsync();
    return;
}

// Each scenario uses an in-memory SQLite DB (named, shared-cache) so state never
// bleeds between runs. RabbitMQ is the real container (spike-rabbitmq, port 5672).

var start = DateTime.UtcNow;
void Log(string msg) => Console.WriteLine($"[T+{(DateTime.UtcNow - start).TotalSeconds:F2}s] {msg}");
void Header(string title) { Console.WriteLine(); Console.WriteLine($"=== {title} ==="); start = DateTime.UtcNow; }
void Pass(string msg) => Console.WriteLine($"Success: {msg}");
void Fail(string msg) { Console.WriteLine($"Error: {msg}"); Environment.Exit(1); }

// ──────────────────────────────────────────────────────────────────────────────
Header("SCENARIO 1: Happy Path");
// ──────────────────────────────────────────────────────────────────────────────
{
    using var repo = new OutboxRepository("Data Source=s1;Mode=Memory;Cache=Shared");
    repo.InitSchema();

    var id = "order-s1-" + Guid.NewGuid().ToString("N")[..8];
    var payload = $"{{\"OrderGuid\":\"{id}\",\"Total\":99.90}}";

    await repo.InsertAsync(id, payload);
    Log($"INSERT {id} (Status=Pending)");

    using var publisher = new RabbitPublisher();
    var dispatcher = new OutboxDispatcher(repo, publisher, Log);

    using var cts = new CancellationTokenSource();
    var task = Task.Run(() => dispatcher.RunAsync(cts.Token));

    await Task.Delay(2500);
    cts.Cancel();
    await task;

    var pending = await repo.CountByStatusAsync("Pending");
    var sent    = await repo.CountByStatusAsync("Sent");

    if (sent == 1 && pending == 0)
        Pass($"QAS-1 (happy path): row published and marked Sent within 2s");
    else
        Fail($"QAS-1 (happy path): expected Sent=1 Pending=0, got Sent={sent} Pending={pending}");
}

// ──────────────────────────────────────────────────────────────────────────────
Header("SCENARIO 2: Broker Down → Recovery");
// ──────────────────────────────────────────────────────────────────────────────
{
    using var repo = new OutboxRepository("Data Source=s2;Mode=Memory;Cache=Shared");
    repo.InitSchema();

    // Stop broker BEFORE inserting rows
    Log("Stopping spike-rabbitmq...");
    await RunShellAsync("docker stop spike-rabbitmq");
    Log("spike-rabbitmq stopped");

    // Insert 3 rows while broker is down — simulates orders committed during outage
    for (int i = 1; i <= 3; i++)
    {
        var id = $"order-s2-{i:D2}-" + Guid.NewGuid().ToString("N")[..6];
        await repo.InsertAsync(id, $"{{\"OrderGuid\":\"{id}\",\"Seq\":{i}}}");
        Log($"INSERT {id} (Status=Pending) — broker is DOWN");
    }

    using var publisher = new RabbitPublisher();
    var dispatcher = new OutboxDispatcher(repo, publisher, Log);

    using var cts = new CancellationTokenSource();
    var task = Task.Run(() => dispatcher.RunAsync(cts.Token));

    // Let dispatcher attempt a few ticks while broker is still down
    await Task.Delay(4000);

    var pendingMid = await repo.CountByStatusAsync("Pending");
    Log($"After 4s with broker DOWN: {pendingMid} row(s) still Pending (expected 3)");

    // Restart broker
    Log("Starting spike-rabbitmq...");
    await RunShellAsync("docker start spike-rabbitmq");
    Log("Waiting for broker to become healthy...");
    await WaitForRabbitAsync(maxWaitSeconds: 30);
    Log("spike-rabbitmq healthy");

    // Wait for dispatcher to recover and drain the queue
    await Task.Delay(5000);
    cts.Cancel();
    await task;

    var pending = await repo.CountByStatusAsync("Pending");
    var sent    = await repo.CountByStatusAsync("Sent");

    if (sent == 3 && pending == 0)
        Pass("QAS-1 (broker-down): all 3 rows recovered and published after broker restart");
    else
        Fail($"QAS-1 (broker-down): expected Sent=3 Pending=0, got Sent={sent} Pending={pending}");
}

// ──────────────────────────────────────────────────────────────────────────────
Header("SCENARIO 3: Process Crash Simulation");
// ──────────────────────────────────────────────────────────────────────────────
{
    // Use a file-based SQLite DB so "crash" and "restart" share state
    var dbPath = Path.Combine(Path.GetTempPath(), $"outbox_spike_{Guid.NewGuid():N}.db");
    var connStr = $"Data Source={dbPath}";

    try
    {
        // --- "Before crash": write rows, never start dispatcher ---
        using var repoPre = new OutboxRepository(connStr);
        repoPre.InitSchema();

        for (int i = 1; i <= 2; i++)
        {
            var id = $"order-s3-{i:D2}-" + Guid.NewGuid().ToString("N")[..6];
            await repoPre.InsertAsync(id, $"{{\"OrderGuid\":\"{id}\",\"Seq\":{i}}}");
            Log($"INSERT {id} (Status=Pending) — dispatcher NOT started (simulates crash)");
        }

        // Simulate process crash: close repoPre (anchor conn drops, but file persists)
        repoPre.Dispose();

        // --- "After restart": fresh repo + dispatcher picks up orphaned rows ---
        Log("Fresh dispatcher started (simulates process restart)");
        using var repoPost = new OutboxRepository(connStr);
        using var publisher = new RabbitPublisher();
        var dispatcher = new OutboxDispatcher(repoPost, publisher, Log);

        using var cts = new CancellationTokenSource();
        var task = Task.Run(() => dispatcher.RunAsync(cts.Token));

        await Task.Delay(2500);
        cts.Cancel();
        await task;

        var pending = await repoPost.CountByStatusAsync("Pending");
        var sent    = await repoPost.CountByStatusAsync("Sent");

        if (sent == 2 && pending == 0)
            Pass("QAS-1 (crash recovery): both rows survived and were published after restart");
        else
            Fail($"QAS-1 (crash recovery): expected Sent=2 Pending=0, got Sent={sent} Pending={pending}");
    }
    finally
    {
        if (File.Exists(dbPath)) File.Delete(dbPath);
    }
}

Console.WriteLine();
Console.WriteLine("All scenarios complete. spike-rabbitmq left running.");

// ──────────────────────────────────────────────────────────────────────────────
static async Task RunShellAsync(string command)
{
    var parts = command.Split(' ', 2);
    var psi = new System.Diagnostics.ProcessStartInfo(parts[0], parts.Length > 1 ? parts[1] : "")
    {
        RedirectStandardOutput = true,
        RedirectStandardError  = true,
        UseShellExecute        = false,
    };
    using var proc = System.Diagnostics.Process.Start(psi)!;
    await proc.WaitForExitAsync();
}

static async Task RunLiveModeAsync()
{
    Console.WriteLine();
    Console.WriteLine("╔══════════════════════════════════════════════════╗");
    Console.WriteLine("║           LIVE MODE — manual demo                ║");
    Console.WriteLine("║  Orders arrive every 3s                          ║");
    Console.WriteLine("║  Stop broker:   docker stop spike-rabbitmq       ║");
    Console.WriteLine("║  Start broker:  docker start spike-rabbitmq      ║");
    Console.WriteLine("║  Exit:          Ctrl+C                           ║");
    Console.WriteLine("╚══════════════════════════════════════════════════╝");
    Console.WriteLine();

    // File-based DB so state is visible across restarts
    var dbPath = Path.Combine(Path.GetTempPath(), "outbox_live.db");
    var repo = new OutboxRepository($"Data Source={dbPath}");
    repo.InitSchema();

    using var publisher = new RabbitPublisher();
    var dispatcher = new OutboxDispatcher(repo, publisher,
        msg => Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] {msg}"));

    using var cts = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

    // Dispatcher runs in background
    var dispatchTask = Task.Run(() => dispatcher.RunAsync(cts.Token));

    // Orders arrive every 3 seconds
    int orderNum = 1;
    while (!cts.IsCancellationRequested)
    {
        var id = $"order-live-{orderNum:D3}-{Guid.NewGuid().ToString("N")[..6]}";
        await repo.InsertAsync(id, $"{{\"OrderGuid\":\"{id}\",\"OrderNum\":{orderNum}}}");

        var pending = await repo.CountByStatusAsync("Pending");
        var sent    = await repo.CountByStatusAsync("Sent");
        Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] ORDER #{orderNum:D3} inserted | Pending={pending} Sent={sent}");

        orderNum++;

        try { await Task.Delay(3000, cts.Token); }
        catch (OperationCanceledException) { break; }
    }

    await dispatchTask;
    Console.WriteLine();
    Console.WriteLine("Live mode stopped.");
}

static async Task WaitForRabbitAsync(int maxWaitSeconds)
{
    var deadline = DateTime.UtcNow.AddSeconds(maxWaitSeconds);
    while (DateTime.UtcNow < deadline)
    {
        try
        {
            using var publisher = new RabbitPublisher();
            // Attempt a trivial publish to confirm broker is up
            publisher.Publish("healthcheck", "{}");
            return;
        }
        catch
        {
            await Task.Delay(1000);
        }
    }
    throw new TimeoutException($"RabbitMQ did not become healthy within {maxWaitSeconds}s");
}
