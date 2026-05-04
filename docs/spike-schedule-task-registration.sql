-- SQL Script to register the Spike Outbox Publisher Task
-- Run this AFTER the first application startup (after migration creates IntegrationEvent table)
-- Choose the appropriate version for your database:

-- ===== PostgreSQL =====
INSERT INTO "ScheduleTask" ("Name", "Seconds", "Type", "Enabled", "StopOnError", "LastEnabledUtc")
VALUES (
    'Spike - Publish Outbox Events',
    10,
    'Nop.Services.Integration.SpikeOutboxPublisherTask, Nop.Services',
    true,
    false,
    NOW()
);

-- ===== MySQL =====
-- INSERT INTO ScheduleTask (Name, Seconds, Type, Enabled, StopOnError, LastEnabledUtc)
-- VALUES (
--     'Spike - Publish Outbox Events',
--     10,
--     'Nop.Services.Integration.SpikeOutboxPublisherTask, Nop.Services',
--     1,
--     0,
--     UTC_TIMESTAMP()
-- );

-- ===== MSSQL =====
-- INSERT INTO ScheduleTask (Name, Seconds, Type, Enabled, StopOnError, LastEnabledUtc)
-- VALUES (
--     'Spike - Publish Outbox Events',
--     10,
--     'Nop.Services.Integration.SpikeOutboxPublisherTask, Nop.Services',
--     1,
--     0,
--     GETUTCDATE()
-- );

-- VERIFICATION QUERY:
-- SELECT * FROM ScheduleTask WHERE Name LIKE '%Spike%';
