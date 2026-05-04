# Feasibility Spike - Execution Guide

## Prerequisites

1. **Start RabbitMQ + Database:**
   ```bash
   docker-compose up -d rabbitmq nopcommerce_database
   ```
   
   Or manually:
   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```
   
   Verify RabbitMQ at: http://localhost:15672 (username: guest, password: guest)

2. **Database connection configured** in nopCommerce

## Execution Steps

### Step 1: Build the Solution

```bash
cd AS-2-NopCommerce
dotnet restore src
dotnet build --no-restore src
```

### Step 2: Run nopCommerce

```bash
cd src/Presentation/Nop.Web
dotnet run
```

**Expected:** Application starts, migration runs, `IntegrationEvent` table is created.

### Step 3: Verify Migration

Connect to your database and check:

```sql
-- Check if table exists
SELECT * FROM IntegrationEvent;

-- Should be empty at this point
```

### Step 4: Register the Schedule Task

Run the SQL script appropriate for your database from `docs/spike-schedule-task-registration.sql`

**For PostgreSQL:**
```sql
INSERT INTO "ScheduleTask" ("Name", "Seconds", "Type", "Enabled", "StopOnError", "LastEnabledUtc")
VALUES (
    'Spike - Publish Outbox Events',
    10,
    'Nop.Services.Integration.SpikeOutboxPublisherTask, Nop.Services',
    true,
    false,
    NOW()
);
```

**Verify:**
```sql
SELECT * FROM "ScheduleTask" WHERE "Name" LIKE '%Spike%';
```

### Step 5: Restart nopCommerce

Stop the application (Ctrl+C) and restart:

```bash
dotnet run
```

**What happens:**
1. `AppStartedEvent` is published
2. `AppStartedEventConsumer` writes a test event to `IntegrationEvent` table
3. `SpikeOutboxPublisherTask` runs every 10 seconds
4. Task reads unpublished events and publishes to RabbitMQ
5. Events are marked as published in database

### Step 6: Verify in Database

```sql
SELECT * FROM "IntegrationEvent" ORDER BY "Id" DESC;
```

**Expected result:**
- At least one row with `EventType = 'ApplicationStarted'`
- `Published = true`
- `PublishedOnUtc` is populated
- `PublishAttempts = 1`

### Step 7: Verify in RabbitMQ Management UI

1. Go to http://localhost:15672
2. Login with guest/guest
3. Click **Exchanges** tab
4. Look for `verdemart.events` exchange (should exist with type=topic)
5. Click **Queues** tab
6. Click "Add a new queue" → Name it `spike-test` → Create
7. Click the queue → "Bindings" section
8. Bind to `verdemart.events` with routing key `#` (matches all)
9. Check "Get messages" → you should see the ApplicationStarted event

### Step 8: Check Logs

Navigate to Admin area (if installed) → System → Log

**Expected log entries:**
- "Spike: AppStartedEvent received, writing to outbox"
- "Spike: Wrote IntegrationEvent to outbox (Id=1)"
- "Spike: Outbox publisher task started"
- "Spike: Found 1 unpublished events"
- "Spike: Published message to RabbitMQ: exchange=verdemart.events, routingKey=ApplicationStarted"
- "Spike: Published IntegrationEvent Id=1"
- "Spike: Outbox publisher task completed"

## Success Criteria (from risk-plan.md)

**Message appears in RabbitMQ within 10s of startup**  
**Row marked as `Published = true` in database**  
**No DI registration errors or crashes**

## Troubleshooting

### Issue: Migration doesn't run
- **Check:** Migration timestamp is 2026-05-04 (future dated)
- **Fix:** Delete the migration file and recreate with correct timestamp
- **Verify:** Check `VersionInfo` table in database

### Issue: Task never executes
- **Check:** SQL insert was successful
- **Check:** `Enabled = true` in ScheduleTask table
- **Fix:** Restart application after SQL insert

### Issue: RabbitMQ connection fails
- **Check:** Docker container is running: `docker ps`
- **Check:** Port 5672 is accessible
- **Fix:** `docker start rabbitmq` or recreate container

### Issue: DI resolution error
- **Check:** `IntegrationStartup.Order = 3000` (must be > 2000)
- **Check:** All services are registered as `Scoped`
- **Fix:** Verify DI registration in IntegrationStartup.cs

### Issue: Event not written to outbox
- **Check:** Logs for "Spike: AppStartedEvent received"
- **Check:** `AppStartedEventConsumer` class exists
- **Fix:** Rebuild solution to register consumer

### Issue: RabbitMQ.Client not found
- **Check:** Nop.Services.csproj has the package reference
- **Fix:** Run `dotnet restore src`

## Clean Database Query

If you want to reset and test again:

```sql
DELETE FROM "IntegrationEvent";
-- Restart application to trigger AppStartedEvent again
```

## Next Steps After Successful Spike

If all verification passes:
1. Mark Risk 1 as MITIGATED in risk-plan.md
2. Document spike results in docs/evidence/
3. Refactor spike code for production:
   - Remove "Spike" prefix from classes
   - Move RabbitMQ config to appsettings.json
   - Add Polly retry policies
   - Replace AppStartedEventConsumer with real OrderPlacedEvent consumer
4. Proceed with full Part 2 implementation
