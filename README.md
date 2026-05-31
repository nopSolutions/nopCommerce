# VerdeMart — Setup and Run Instructions

Architecture report: [`docs/architecture/architecture-report.md`](docs/architecture/architecture-report.md)

---

## Prerequisites

- Docker Desktop

---

## Start Everything

```bash
docker-compose up -d
```

To rebuild after source code changes:

```bash
docker-compose up -d --build
```

---

## First Run — nopCommerce Setup

On first run, open `http://localhost:80` — the installation wizard will appear.

Check **"Enter raw connection string"** and use:

```
Server=nopcommerce_mssql_server;Database=nopcommerce_mssql_server;User Id=sa;Password=nopCommerce_db_password;
```

Complete the wizard. On the admin credentials step, set the email and password you will use to log in to the admin panel.

---

## Service URLs

These are browser URLs (host machine). Plugin configuration fields use Docker internal hostnames instead — see below.

| Service | URL | Credentials |
|---------|-----|-------------|
| nopCommerce storefront | `http://localhost:80` | customer account |
| nopCommerce admin | `http://localhost:80/admin` | set during wizard |
| RabbitMQ management | `http://localhost:15672` | guest / guest |
| OpenBoxes | `http://localhost:8080/openboxes` | admin / admin |
| WireMock | `http://localhost:8090` | — |

---

## Plugin Activation

After the wizard, the VerdeMart plugins must be installed and the application restarted before the architecture is active. Without this step the outbox, pollers, allocation gate, and carrier integration will not run.

**1. Go to Admin → Configuration → Local plugins**

**2. Find and click Install for each of the following:**

- `Nop.Plugin.Messaging.RabbitMq`
- `Nop.Plugin.Inventory.AllocationGate`
- `Nop.Plugin.Integration.Pos`
- `Nop.Plugin.Shipping.CarrierTracking`
- `Nop.Plugin.Fulfillment.OpenBoxes`

**3. Restart the application**

After installing all plugins, restart nopCommerce:

```bash
docker-compose restart nopcommerce_web
```

The application must be restarted for the scheduled tasks (outbox dispatcher, pollers) and the allocation gate decorator to be registered and active.

---

## Before Running the Use Cases — Seed OpenBoxes Stock

OpenBoxes must have products and stock before any fulfillment order can be created. Run this once after the stack is up and plugins are installed:

```bash
python3 scripts/init_openboxes_stock.py
```

This reads all nopCommerce sample products from `SampleData.json` and creates them in OpenBoxes with initial stock.

---

## Demonstration Use Cases

### Use Case 1 — Buy online, fulfill through another channel

An order placed on the nopCommerce web storefront is automatically routed to OpenBoxes for warehouse fulfillment and then to the carrier for shipping.

**What to do:**
1. Place an order as a customer on the storefront (`http://localhost:80`)
2. Open OpenBoxes (`http://localhost:8080/openboxes`) — the fulfillment order appears within seconds (created by the OpenBoxes Bridge consuming the RabbitMQ event)
3. In OpenBoxes, go to **Outbound → List Outbound Movements** — find the created order, then:
   - Click **Edit**
   - Click **Next**
   - Click **Next**
   - Select a **Shipment Type** and an **Expected Delivery Date**
   - Click **Send Shipment**
4. Within 30 seconds, nopCommerce reflects this: the order status becomes `Complete`, a `Shipment` is created, and a carrier booking is sent to WireMock automatically

**Where to observe:**
- RabbitMQ management (`http://localhost:15672`) → queues `verdemart.orders.openboxes` and `carrier.booking.requested` draining
- Bridge logs → confirm the `order.placed` event was consumed and the fulfillment order was created in OpenBoxes:
  ```bash
  docker logs verdemart_openboxes_bridge
  ```
- nopCommerce admin → Orders → order status changes to `Complete`

---

### Use Case 2 — Cross-channel state visibility

A status change that happens outside nopCommerce (in OpenBoxes or the carrier) becomes visible inside nopCommerce without any operator action.

**Order state progression:**

| Step | Trigger | OrderStatus | ShippingStatus | Carrier Status (shipment UI) |
|------|---------|-------------|----------------|------------------------------|
| Order placed | Customer checkout | Processing | Not yet dispatched | — |
| OpenBoxes Bridge creates fulfillment | RabbitMQ event consumed | Processing | Not yet dispatched | — |
| OpenBoxes fulfillment marked `SHIPPED` | `OpenBoxesStatusPollerTask` detects it (≤ 30 s) | **Complete** | Not yet dispatched | **Pending Dispatch** |
| Carrier booking confirmed | `CarrierBookingConsumer` calls WireMock | Complete | Not yet dispatched | **Dispatched** |
| WireMock status → `IN_TRANSIT` | `CarrierStatusPollerTask` detects it (≤ 30 s) | Complete | **Dispatched** | **In Transit** |
| WireMock status → `OUT_FOR_DELIVERY` | `CarrierStatusPollerTask` detects it (≤ 30 s) | Complete | Dispatched | **Out for Delivery** |
| WireMock status → `DELIVERED` | `CarrierStatusPollerTask` detects it (≤ 30 s) | Complete | **Delivered** | **Delivered** |

**What to do:**
1. After Use Case 1, WireMock now holds an active shipment with a tracking ID
2. Update the shipment status in WireMock (e.g. to `IN_TRANSIT` or `DELIVERED`)
3. Within 30 seconds, nopCommerce reflects the new carrier status on the shipment and sends a customer notification email

**Where to observe:**
- nopCommerce admin → Orders → Shipments → `ExternalShippingStatus` field updated or cient  -> My Account → Orders → Order details
- nopCommerce admin → System → Schedule tasks → confirm `OpenBoxesStatusPollerTask` and `CarrierStatusPollerTask` last run times
- WireMock logs → confirm the carrier booking request was received:
  ```bash
  docker logs wiremock
  ```

---

## Stopping

```bash
docker-compose down
```

To remove all data volumes (full reset):

```bash
docker-compose down -v
```
