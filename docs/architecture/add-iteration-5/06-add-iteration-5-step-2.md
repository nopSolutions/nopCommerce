# ADD Iteration 5 — Step 2: Establish Iteration Goal and Select Element to Decompose

## Element Selected

**The gap between OpenBoxes fulfillment state and nopCommerce order status.**

Currently, when the bridge consumer creates a fulfillment order in OpenBoxes and OpenBoxes subsequently processes it to `ISSUED`, that state change is invisible to nopCommerce. The order in nopCommerce remains in its post-placement state indefinitely — no automated mechanism exists to observe the warehouse lifecycle and reflect it internally.

---

## Why This Element

This is the only gap remaining in QAS-5. The carrier path (Iter 4) handles the shipping leg. The warehouse leg — OpenBoxes picking, packing, and issuing the order — produces no signal that nopCommerce can receive. The customer-facing order detail page cannot show fulfillment progress without this.

The element to decompose is the **nopCommerce side of the warehouse observation loop**: a component that periodically queries OpenBoxes for fulfillment state changes and applies them to the relevant nopCommerce orders.

---

## Scope Boundary

| In scope | Out of scope |
| --- | --- |
| Detecting `ISSUED` state in OpenBoxes and updating nopCommerce order status | Full warehouse lifecycle (PICKING, VERIFYING, etc.) beyond ISSUED |
| Correlating OpenBoxes fulfillment orders back to nopCommerce orders via `OrderGuid` | Pushing additional data (line-item detail, warehouse notes) back to nopCommerce |
| Triggering the carrier booking automatically on `ISSUED` detection | Replacing the existing manual carrier booking trigger |
| Configurable polling interval | Real-time push (not available from OpenBoxes) |

Step 3 evaluates the candidate design concepts for this element.
