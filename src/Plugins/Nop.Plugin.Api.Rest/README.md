Nop.Plugin.Api.Rest

A nopCommerce plugin that exposes REST API endpoints for the catalog, orders, customers, shipments, logs,
and a storefront-facing product view. It also issues customer tokens, so a client can act on one
customer's own data without holding the admin API key.

Installation
1. Add the project to the solution: dotnet sln src\NopCommerce.sln add Plugins\Nop.Plugin.Api.Rest\Nop.Plugin.Api.Rest.csproj
2. Build the solution.
3. In the nopCommerce admin, install and enable the plugin (Configuration -> Local plugins).
4. Open the plugin configuration page (Configuration -> Local plugins -> Configure) and generate an API key.

Plugin-contained Swagger
- Swagger UI: /swagger/api-rest/index.html
- Backend JSON: /swagger/api-backend/swagger.json
- Public store JSON: /swagger/api-frontend/swagger.json

The API is published as **two documents**, split the way the official nopCommerce Web API splits itself:
the back office operations in the backend document, the public store operations in the public store
document. The Swagger UI page lists both in a "Select a definition" dropdown, so there is one URL to
bookmark rather than two pages to choose between.

Each document's description block is that side's credential guide, and each publishes only the
credentials it accepts. The **backend** document lists two schemes in the Authorize dialog, `ApiKey` and
`Bearer`; the **public store** document lists `Bearer` alone, because its operations refuse the API key
with 403 and advertising it would promise something the runtime rejects.

## Two credentials, two sides

The plugin accepts two kinds of credential. Each has exactly one header it is valid in, and neither is
accepted in the other's place.

**A bearer token** is a JWT. You get one by posting credentials to the token endpoint for your side, and
you send it as:

    Authorization: Bearer <token>

A token is either admin level or scoped to one customer, and the request is told which. An admin token
grants access to every record; use it for back-office and integrations. A customer token grants access
to that customer's own data only; use it for storefronts, mobile apps and anything acting on behalf of a
shopper.

**The shared API key** is a static secret you configure on the plugin settings page. It is valid only in:

    X-Api-Key: <API_KEY>

It grants admin level access to every record, the same power as an admin token, and never expires. Use
it for server-to-server calls that would rather not hold a short-lived token.

A missing or wrong credential is refused with 401. There is no session or cookie authentication: being
signed in to the admin area does not authenticate an API request.

## Two sides, and the credentials they accept

The API splits into a back office and a public store, following the same division the official API makes
between `api-backend` and `api-frontend`. **The scopes are kept apart in both directions**, because both
tokens are signed with the same key and the scope in the token is otherwise just a label:

| Side | Routes | Accepted |
|---|---|---|
| **Back office** | `/api/rest/products`, `/orders`, `/shipments`, `/customers`, `/categories`, `/manufacturers`, `/logs`, `/sales`, `/metafields` and their subroutes | API key or admin token. **A customer token gets 403.** |
| **Public store, personal** | `/api/rest/customer/me/*` | Customer token, always. **The API key and an admin token both get 403.** |
| **Public store, catalog** | `/api/rest/store/*` | Anonymous while "Require a credential for reads" is off; a customer token once it is on. **The API key and an admin token always get 403.** |
| **Token endpoints** | all two token URLs | none; they are how you get a credential |

The catalog projection is not treated like the personal routes on purpose: it is what an anonymous
visitor browses, so it follows the read setting. Both public store paths still refuse an admin level
credential outright, because a storefront client has no business holding the shared API key, and
shipping it to a browser or a phone is how it ends up in the wild.

Which side a route belongs to is decided once, in `ApiRestDefaults.IsFrontendPath`, and that single
predicate drives the middleware, the Swagger split and the published security requirement — so the
documented contract cannot promise a credential the runtime then refuses.

## Getting an admin token

Exchange administrator credentials for one. Only a member of the Administrators role is served.

    POST /api/rest/token
    { "email": "admin@example.com", "password": "..." }

The response carries the token, how long it stays valid, and which HTTP methods need a credential at all.
The token is valid until it expires; there is no refresh, so post the credentials again for a new one.

## Getting a customer token

Exchange store credentials for one. Registered customers only; guests and administrators are refused.

    POST /api/rest/customer/token
    { "email": "customer@example.com", "password": "..." }

Send `username` instead of `email` on any store that has usernames enabled. **Either one on its own is
enough** — send the address alone on a store without usernames, or the username alone on a store with
them. Only a request carrying neither is refused, and then with 400 from the action rather than from
model validation.

## Token lifetimes

Both lifetimes are configurable on the plugin settings page: **Admin token lifetime (hours)**, default 24,
and **Customer token lifetime (days)**, default 7. The admin default is the shorter of the two because an
admin token grants catalog, order and customer write access, while a customer token is scoped to one
customer's own data.

Changing a lifetime only affects tokens issued afterwards. Tokens are signed with the shared API key, so
**regenerating the API key invalidates every token already issued**.

Both token endpoints are exempt from the credential check, because they are how a credential is obtained
in the first place. The rate limit still applies to all of them, which is what protects them from
brute forcing.

In the Swagger UI click Authorize and paste the token. The admin cookie is not used, and the customer
scoped operations expect a customer token rather than the shared API key.

## Authorization rules

| Route | Credential |
|---|---|
| `/api/rest/token`, `/api/rest/customer/token` | none |
| `/api/rest/customer/me/*` | customer token, always; the API key and an admin token both get 403 |
| `/api/rest/store/*` | anonymous while reads are unprotected, otherwise a customer token; an admin level credential always gets 403 |
| `/api/rest/products*`, `/orders*`, `/shipments*`, `/customers*`, `/categories*`, `/manufacturers*`, `/logs*`, `/sales*`, `/metafields*` | API key or admin token; a customer token gets 403 |
| reads, within the back office | anonymous when "Require a credential for reads" is off, otherwise a credential |

Every `me` route takes the customer from the credential itself. There is no customer parameter anywhere
in their query strings, paths or bodies, so a caller cannot reach another customer's data by changing a
value.

## Configuration

Settings live on the plugin configuration page and are stored per store, so each tenant can hold its own
key.

- API key: generated on the configuration page, or written there directly. Must be at least 32 characters,
  because it also signs bearer tokens. Use the generate button rather than typing a short key.
- Require a credential for reads: when enabled, anonymous callers get 401 on the back office reads unless they present the API key or a bearer token. Leave it off to browse the catalog anonymously, but note the back office customer and order reads are then public too. `/api/rest/customer/me/*` is never public, and `/api/rest/store/*` moves from anonymous to needing a customer token — the API key is refused there either way.
- Rate limit per minute: requests allowed per client per minute, default 60.
- Admin token lifetime / Customer token lifetime: how long each kind of issued token stays valid.

Note that leaving the read setting disabled also exposes customer and order reads, which contain personal
data. Enable it for any store reachable from outside a trusted network.

The Swagger document mirrors these rules per operation, and the customer scoped operations are published
with the bearer token scheme, because they refuse the API key and any admin token. `POST /api/rest/token`
reports the guarded methods in its `SecuredMethods` field.

## Paging

List endpoints return an envelope rather than a bare array, so a client can see how many records matched
without fetching every page:

    {
      "items": [ ... ],
      "pageIndex": 0,
      "pageSize": 20,
      "totalCount": 137,
      "totalPages": 7
    }

`pageIndex` is zero based. `pageSize` defaults to 20 and is capped at 200. A `pageIndex` below zero is
treated as 0 and a `pageSize` of 0 or less as the default.

## Endpoints

### Customer identity
- `POST /api/rest/token` - administrator credentials, returns an admin bearer token
- `POST /api/rest/customer/token` - customer credentials, returns a customer token
- `GET /api/rest/customer/me` - the token holder's own profile
- `GET /api/rest/customer/me/addresses` - their addresses
- `GET /api/rest/customer/me/orders` - their orders; filters: orderStatusId, paymentStatusId, shippingStatusId
- `GET /api/rest/customer/me/wishlists` - their named wishlists
- `GET /api/rest/customer/me/wishlist` - their wishlist lines; `wishlistId` narrows to one named wishlist

### Catalog
- `GET /api/rest/products` - filters: keywords, categoryId, manufacturerId, priceMin, priceMax, searchSku, searchDescriptions
- `GET /api/rest/products/{id}` - full product as ProductDetailDto
- `GET /api/rest/products/attributes/{productId}` - attribute mappings with their selectable values
- `POST /api/rest/products` - create; Name is required
- `PATCH /api/rest/products/{id}` - updates the supplied properties only, so a string field is cleared by sending "" rather than null
- `DELETE /api/rest/products/{id}` - soft delete, returns 204
- `GET /api/rest/categories` - filters: name, showHidden
- `GET /api/rest/categories/{id}`
- `GET /api/rest/manufacturers` - filters: name
- `GET /api/rest/manufacturers/{id}`
- `GET /api/rest/store/products/{id}` - the storefront view of a product, with its variants, manufacturers and tags. Unpublished, deleted, or not-mapped-to-the-requested-store products return 404, so the storefront cannot tell the difference. Send `X-Store-Id: <id>` to pick a store on a multi-store install; omitting it leaves the store unfiltered.

### Orders
- `GET /api/rest/orders` - filters: customerId, billingEmail, billingPhone, orderStatusId, paymentStatusId, shippingStatusId, createdFromUtc, createdToUtc
- `GET /api/rest/orders/{id}` - full detail with items, billing and shipping address
- `GET /api/rest/orders/search?orderNumber=1003` - by the number a merchant assigned to the order
- `GET /api/rest/orders/{id}/shipments`
- `POST /api/rest/orders/{id}/mark-paid`
- `POST /api/rest/orders/{id}/cancel` - body: `{ "notifyCustomer": true }`
- `POST /api/rest/orders/{id}/shipments` - body: `{ "items": [ { "orderItemId": 1, "quantity": 2 } ], "trackingNumber": "..." }`; omit `items` to ship everything still pending

### Shipments
- `GET /api/rest/shipments?orderId=1`
- `GET /api/rest/shipments/{id}`
- `POST /api/rest/shipments/{id}/ship` - body: `{ "notifyCustomer": true }`

### Customers (admin view)
- `GET /api/rest/customers` - filters: email, firstName, lastName, phone, customerRoleId, createdFromUtc, createdToUtc, isActive
- `GET /api/rest/customers/{id}`
- `GET /api/rest/customers/{id}/addresses`

### Other
- `GET /api/rest/sales/totals`
- `GET /api/rest/logs` - filters: message, logLevelId, fromUtc, toUtc
- `GET /api/rest/metafields?entityType=product&entityId=1[&key=]` - generic attributes. `entityType` accepts a friendly name (product, customer, category, manufacturer, order, address, vendor) and is mapped to the key group nopCommerce stores it under. Read only by design: a write here would allow attaching an arbitrary attribute to any entity, a far broader capability than the rest of this API's write surface.

## Coverage

[API-COVERAGE.md](API-COVERAGE.md) tracks this surface against the two official nopCommerce Web API
specifications and lists what is still to come.

## Postman collection

A sample collection is at Plugins/Nop.Plugin.Api.Rest/postman/Nop.Plugin.Api.Rest.postman_collection.json.
Set the `base_url` and `token` variables before running.

## Notes

- Credential and rate-limit middleware applies to requests under /api/rest only, plus the two official shaped token routes. The plugin Swagger UI is excluded.
- Each credential is valid in one header only. `Authorization: Bearer` carries a signed token and is not accepted for the raw API key; `X-Api-Key` carries the raw API key and is not accepted for a token. A caller holding both cannot present the wrong one by mistake.
- The two token scopes are enforced against each other, not just labelled: a customer token is refused on back office routes and an admin level credential is refused on public store routes. Both tokens are signed with the same API key, so without that check the scope claim would be decorative.
- Rate limited clients are bucketed by the scope they proved: each customer token holder gets their own bucket, and admin level callers — whether they presented the API key or an admin token — share one. Requests without a valid credential are bucketed per IP.
- Log records are readable at /api/rest/logs, and a failed record carries its full exception message. Enable the read setting to keep that behind the credential.
- MFA cannot be used through the API: there is no way to complete an interactive challenge, so such accounts are refused at the token endpoints.
- Avoid returning domain entities directly; the plugin uses DTOs to define the public contract.
