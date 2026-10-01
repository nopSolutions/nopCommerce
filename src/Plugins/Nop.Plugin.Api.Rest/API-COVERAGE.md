# API coverage tracker

Tracks this plugin's REST surface against the two official nopCommerce Web API specifications, so the
remaining work is a known list rather than something to rediscover each time.

## Reference sources

| Suite | File | Swagger UI |
|---|---|---|
| Backend (admin area) | `D:\PROJECTS\miscNopCommerce\nopcommerceBackend.json` (19 MB) | https://demo.nopcommerce.com/api/index.html?urls.primaryName=nopCommerce+Web+API+for+backend+v4.90.1 |
| Frontend (public store) | `D:\PROJECTS\miscNopCommerce\nopcommerceFrontend.json` (5 MB) | https://demo.nopcommerce.com/api/index.html?urls.primaryName=nopCommerce+Web+API+for+public+store+v4.90.1 |

**Provenance.** The counts below were read out of those two files, not recalled. Backend:
1,011 operations across 152 controllers. Frontend: 221 operations across 26 controllers. The official
plugin is a commercial nopCommerce product; its source is not part of the nopCommerce repository, so
this tracker is the only local record of what it covers.

## Conventions

The official API routes as `/api-backend/{Controller}/{Action}` and `/api-frontend/{Controller}/{Action}`.
This plugin does not mirror those shapes. It uses resource-oriented routes under a single `/api/rest`
prefix, so the mapping below is by capability rather than by path.

| Symbol | Meaning |
|---|---|
| DONE | Implemented |
| PARTIAL | Some operations implemented |
| TODO | Not implemented |
| N/A | Deliberately out of scope, reason given |

## Status legend

`—` in the "Official route" column means the capability is not exposed by the official API under a
single named action, and this plugin offers it anyway.

---

## Phase 0 — Foundation (DONE)

| Area | Official route | Our route | Status |
|---|---|---|---|
| Admin token | `api-backend/Authenticate/GetToken` | `POST /api/rest/token` | DONE |
| Customer token | `api-frontend/Authenticate/GetToken` | `POST /api/rest/customer/token` | DONE |
| Credential slots | — | `Authorization: Bearer` = token, `X-Api-Key` = key | DONE |
| Backend / public store split | `api-backend/*` vs `api-frontend/*` | two Swagger documents, scopes enforced both ways | DONE |
| Token lifetimes | — | configurable per scope | DONE |
| Read protection | — | `RequireApiKeyForReads` setting | DONE |
| Rate limiting | — | per scope and per IP | DONE |
| Paging envelope | — | `PagedResult<T>` | DONE |

One token format serves both scopes. A bearer token issued to an administrator grants admin level access;
one issued to a customer is scoped to that customer's own data. The scopes are enforced against each
other rather than merely labelled: a customer token is refused on back office routes, and an admin level
credential is refused on public store routes. Both are signed with the same API key, so without that check
the scope claim would carry no authority.

The split is published as two Swagger documents, `api-backend` and `api-frontend`, chosen from a "Select
a definition" dropdown on one UI page, matching how the official API presents itself. The document names
match the official suite's, but the routes below them do not: this plugin serves everything under
`/api/rest` and does not also answer at the official path spellings. The capability mapping in the tables
below is therefore by behaviour, not by path.

## Phase 1 — Core read (DONE)

| Area | Official route | Our route | Status |
|---|---|---|---|
| Products | `api-backend/Product/GetAll` | `GET /api/rest/products` | DONE |
| Product detail | `api-backend/Product/GetById` | `GET /api/rest/products/{id}` | DONE |
| Categories | `api-backend/Category/GetAll` | `GET /api/rest/categories` | DONE |
| Manufacturers | `api-backend/Manufacturer/GetAll` | `GET /api/rest/manufacturers` | DONE |
| Product attributes | `api-backend/ProductAttributeMapping` | `GET /api/rest/products/attributes/{productId}` | DONE |
| Orders | `api-backend/Order/GetAll` | `GET /api/rest/orders` | DONE |
| Order detail | `api-backend/Order/GetById` | `GET /api/rest/orders/{id}` | DONE |
| Order by number | — | `GET /api/rest/orders/search?orderNumber=` | DONE |
| Customers | `api-backend/Customer/GetAll` | `GET /api/rest/customers` | DONE |
| Customer addresses | `api-backend/Address` | `GET /api/rest/customers/{id}/addresses` | DONE |
| Logs | — | `GET /api/rest/logs` | DONE |
| Metafields | `api-backend/GenericAttribute` | `GET /api/rest/metafields` | DONE (read only) |
| Sales report | `api-backend/OrderReport` | `GET /api/rest/sales/totals` | DONE |

## Phase 2 — Order and shipment writes (DONE)

| Area | Official route | Our route | Status |
|---|---|---|---|
| Mark order paid | `api-backend/OrderProcessing/MarkOrderAsPaid` | `POST /api/rest/orders/{id}/mark-paid` | DONE |
| Cancel order | `api-backend/OrderProcessing/CancelOrder` | `POST /api/rest/orders/{id}/cancel` | DONE |
| Create shipment | `api-backend/OrderProcessing/Shipment` | `POST /api/rest/orders/{id}/shipments` | DONE |
| Mark shipped | `api-backend/OrderProcessing/Ship` | `POST /api/rest/shipments/{id}/ship` | DONE |
| Shipment reads | `api-backend/Shipment` | `GET /api/rest/shipments`, `/orders/{id}/shipments` | DONE |
| Product create | `api-backend/Product/Create` | `POST /api/rest/products` | DONE |
| Product update | `api-backend/Product/Update` | `PATCH /api/rest/products/{id}` | DONE |
| Product delete | `api-backend/Product/Delete` | `DELETE /api/rest/products/{id}` | DONE |

## Phase 3 — Customer identity and storefront (DONE)

Removes the old `/api/rest/customerproducts` family, which exposed wishlist lines under a name that did
not say what they were.

| Area | Official route | Our route | Status |
|---|---|---|---|
| Customer token | `api-frontend/Authenticate/GetToken` | `POST /api/rest/customer/token` | DONE |
| Own profile | `api-frontend/Customer/GetCurrentCustomer` | `GET /api/rest/customer/me` | DONE |
| Own addresses | `api-frontend/Customer/Addresses` | `GET /api/rest/customer/me/addresses` | DONE |
| Own orders | `api-frontend/Order/GetAll` | `GET /api/rest/customer/me/orders` | DONE |
| Own wishlists | `api-frontend/Wishlist` | `GET /api/rest/customer/me/wishlists` | DONE |
| Own wishlist lines | `api-frontend/Wishlist/Wishlist` | `GET /api/rest/customer/me/wishlist` | DONE |
| Storefront product | `api-frontend/Product/GetProductDetails` | `GET /api/rest/store/products/{id}` | DONE |

## Phase 4 — Catalog and customer breadth (TODO)

The largest remaining gaps, by official operation count. None is started.

| Area | Official ops | Official route | Status | Notes |
|---|---|---|---|---|
| Product (remaining) | 41 total, ~6 done | `api-backend/Product/*` | PARTIAL | related products, new products, product pictures, videos, cross-sell, upsell, availability ranges, warehouse stock |
| OrderProcessing (remaining) | 33 total, 3 done | `api-backend/OrderProcessing/*` | PARTIAL | refund, restore, ready for pickup, deliver, complete, re-order |
| Customer (remaining) | 32 total, 3 done | `api-backend/Customer/*` | PARTIAL | create, update, delete, roles, reward points, password |
| Category (remaining) | 18 total, 2 done | `api-backend/Category/*` | PARTIAL | create, update, delete |
| CustomerRole | 16 | `api-backend/CustomerRole/*` | TODO | |
| Manufacturer (remaining) | 15 total, 2 done | `api-backend/Manufacturer/*` | PARTIAL | create, update, delete |
| Vendor | 12 | `api-backend/Vendor/*` | TODO | multi-vendor support |
| ProductReview | 12 | `api-backend/ProductReview/*` | TODO | |
| ProductTag | 12 | `api-backend/ProductTag/*` | TODO | |
| SpecificationAttribute | 10 | `api-backend/SpecificationAttribute/*` | TODO | |
| Currency | 13 | `api-backend/Currency/*` | TODO | |
| Picture | 13 | `api-backend/Picture/*` | TODO | |
| Discount | 6 | `api-backend/Discount/*` | TODO | |
| ReturnRequest | 5 | `api-backend/ReturnRequest/*` | TODO | |

Security note for when `Customer` writes land: this plugin has a single shared API key with no scopes.
Exposing a customer create or update that accepts a role would let any key holder promote a customer to
Administrators. Role assignment should go behind `CustomerRole` with an explicit gate, not ride along in
the customer payload.

## Phase 5 — Storefront suite (TODO)

Unblocked by the customer token from Phase 3. All of these need a caller identity, which the customer
token now supplies.

| Area | Official ops | Official route | Status |
|---|---|---|---|
| Catalog | 22 | `api-frontend/Catalog/*` | TODO |
| Product (public) | 17 | `api-frontend/Product/*` | PARTIAL (1 of 17) |
| Shopping cart | 15 | `api-frontend/ShoppingCart/*` | TODO |
| Checkout | 20 | `api-frontend/Checkout/*` | TODO |
| Download | 9 | `api-frontend/Download/*` | TODO |
| Private messages | 8 | `api-frontend/PrivateMessages/*` | TODO |
| Boards / forum | 24 | `api-frontend/Boards/*` | TODO |
| Return request | 4 | `api-frontend/ReturnRequest/*` | TODO |
| Back in stock | 4 | `api-frontend/BackInStockSubscription/*` | TODO |

## Out of scope

| Area | Reason |
|---|---|
| Forum / boards | nopCommerce 4.90 requires a third-party forum plugin; the entity does not exist in core |
| `set_metafield` write | Allows attaching an arbitrary attribute to any entity type, which is a far broader capability than the rest of this API's write surface. Revisit with an explicit allowlist of key groups. |
