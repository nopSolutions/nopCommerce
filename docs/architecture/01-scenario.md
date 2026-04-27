# Scenario C - Omnichannel Commerce Core

## VerdeMart

VerdeMart is a retail business that started with a single web storefront. As it grew, the business added a physical warehouse, a store with a point-of-sale terminal, and a relationship with a shipping carrier. Each of these is a separate system, run by different people, with different concerns. The challenge is making them work as one.

nopCommerce is no longer only the online shop. It is becoming the commerce core in a wider enterprise ecosystem.

The challenge is not merely connecting more systems. The challenge is deciding how the commerce core should behave when surrounding systems disagree, lag, degrade, or recover.

## Strategic Goals

1. Make nopCommerce act as a reliable commerce core rather than an isolated storefront.
2. Improve cross-channel visibility of stock, order state, and fulfillment progress.
3. Remain useful when surrounding operational systems are stale, delayed, or temporarily unavailable.

## Surrounding Systems

| System | Role |
|---|---|
| nopCommerce | Commerce core - order state machine, customer-facing experience |
| ERPNext | Financial backbone - sales orders, stock ledger, accounting |
| OpenBoxes | Warehouse - pick/pack/ship, physical inventory truth |
| Open Source POS | In-store sales channel - consumes shared inventory |
| RabbitMQ | Message broker - async coordination between all systems |
| WireMock | Shipping carrier simulator - tracking updates and failure injection |
| Keycloak | Identity - single sign-on across all channels |
| Meilisearch | Product search - catalogue freshness pressure (low priority) |

## Why This Scenario

The business pressure is real and specific: a customer places an order online, but the warehouse may be temporarily unreachable, the ERP may be slow, and the POS may have just sold the last unit of the same product. The architecture must keep the checkout working and eventually deliver a consistent state across all systems - without blocking the customer or losing data.

This creates three concrete architectural tensions:
- **Reliability**: events must not be lost when downstream systems are unavailable
- **Availability**: checkout must complete regardless of ERP or warehouse latency
- **Consistency**: stock sold in one channel must become visible in all channels
