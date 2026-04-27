# ADR-001: Messaging Backbone — RabbitMQ vs Apache Kafka

**Status:** Accepted  
**Date:** 2026-04-26  
**Owner:** Sebastião  
**Deciders:** Full team

---

## Context

Scenario C requires at least one asynchronous workflow and explicit reliability decisions. We need a message broker to decouple nopCommerce order events from ERP and WMS processing.

The two primary candidates for open-source message brokers are **RabbitMQ** and **Apache Kafka**.

---

## Decision

**Use RabbitMQ** as the message broker for the `verdemart.events` topic exchange.

---

## Rationale

| Criterion | RabbitMQ | Kafka |
|-----------|----------|-------|
| Operational complexity | Low — single broker, standard queues, native DLX | High — requires ZooKeeper or KRaft, partition management |
| Native dead-letter support | Yes — Dead Letter Exchanges (DLX) out of the box | No — requires custom offset management or external tooling |
| Message routing flexibility | Yes — topic exchanges, per-message routing keys | No — topics only, no content-based routing |
| Message acknowledgement | Per-message ACK/NACK with requeue | Offset-based; consumer must track position |
| Throughput needs | Sufficient (< 1000 msg/s for this scenario) | Overkill; designed for 100k+ msg/s |
| Docker simplicity | `rabbitmq:management` image, ready in seconds | Multi-container setup |

For our scenario (order-level events, ~10 orders/min demo load), Kafka's strengths (log compaction, massive throughput, consumer group replay) provide no benefit and significantly increase operational overhead. RabbitMQ's dead-letter exchange feature directly supports our mandatory reliability requirement (dead-letter queue for WMS failures).

---

## Rejected Alternative: Apache Kafka

Kafka would require:
- A multi-broker or KRaft setup for any realistic reliability guarantee
- Custom dead-letter handling (Kafka has no native DLX)
- Offset management in the consumer — more complex reconciliation logic

Kafka would be the right choice if we needed event sourcing, long-term event replay, or multi-consumer group fan-out at scale. None of those are required by Scenario C.

---

## Consequences

- RabbitMQ `verdemart.events` exchange (topic type) is the single event backbone
- Dead Letter Exchange `verdemart.dlx` + queue `verdemart.dead-letter` holds undeliverable WMS messages
- All services connect to RabbitMQ using AMQP 0-9-1 protocol
- If future scale requires Kafka, migration path exists: replace the outbox publisher and Integration Service consumer — contract schemas remain unchanged
