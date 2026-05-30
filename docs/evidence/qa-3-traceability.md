# QA-3 Evidence - Plugin-Side Traceability

## Scope

This file records the traceability delivered by the Inbox + POS track. Full order-to-WMS traceability belongs to Phase 5 after the worker and WMS path exist. This track establishes the same message envelope discipline on the plugin receive side.

## Trace Fields

Every POS callback carries and stores:

| Field | Where it appears |
|-------|------------------|
| `messageId` | callback body, `OmniInboxMessage.MessageId`, callback response, `OmniStockSyncState.LastMessageId` |
| `correlationId` | callback body, `OmniInboxMessage.CorrelationId`, callback response |
| `eventType` | callback body, `OmniInboxMessage.EventType` |
| `source` | callback body, `OmniInboxMessage.Source`, `OmniStockSyncState.Source` |
| `sourceVersion` | callback body, `OmniStockSyncState.SourceVersion` |

## Callback Response

The plugin callback returns enough information to connect HTTP response, inbox row and stock projection row:

```json
{
  "result": "applied",
  "messageId": "e2251e6f-6e06-44f2-9c54-6d41f72d7fb8",
  "correlationId": "e2251e6f-6e06-44f2-9c54-6d41f72d7fb8",
  "inboxId": 10,
  "stockSyncStateId": 4,
  "applied": true,
  "duplicate": false,
  "stale": false,
  "sourceVersion": 42
}
```

## Verification Queries

Start from a `messageId` returned by the POS simulator:

```sql
DECLARE @MessageId UNIQUEIDENTIFIER = '<message-id-from-response>';

SELECT
    [Id] AS InboxId,
    [MessageId],
    [CorrelationId],
    [EventType],
    [Source],
    [StatusId],
    [ReceivedOnUtc],
    [ProcessedOnUtc],
    [UpdatedOnUtc]
FROM [dbo].[OmniInboxMessage]
WHERE [MessageId] = @MessageId;
```

For a normal or stale POS event, connect the latest stock projection back to the message:

```sql
DECLARE @MessageId UNIQUEIDENTIFIER = '<message-id-from-response>';

SELECT
    i.[Id] AS InboxId,
    i.[MessageId],
    i.[CorrelationId],
    i.[EventType],
    s.[Id] AS StockSyncStateId,
    s.[ProductId],
    s.[WarehouseId],
    s.[QuantityOnHand],
    s.[SourceVersion],
    s.[StatusId],
    s.[LastSeenOnUtc]
FROM [dbo].[OmniInboxMessage] i
LEFT JOIN [dbo].[OmniStockSyncState] s
    ON s.[LastMessageId] = i.[MessageId]
WHERE i.[MessageId] = @MessageId;
```

For duplicate events, the second HTTP response returns `duplicate` and points back to the existing `inboxId`; no second inbox row is created.

## QA-3 Status

Current status: **partial for plugin receive side; build validated on 2026-05-15**.

The POS path is traceable by `messageId` and `correlationId` across HTTP response, inbox row and stock projection row. Full QA-3 remains open until Phase 5 adds order lookup, worker attempts and fulfillment traceability.
