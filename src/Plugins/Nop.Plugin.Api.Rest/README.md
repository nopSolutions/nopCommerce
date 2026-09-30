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
- Swagger JSON: /swagger/v1/swagger.json

## Two credentials

The plugin accepts two kinds of credential, and the request is told which one it presented.

**The shared API key** grants admin level access to every record. Use it for back-office and
integrations.

    X-Api-Key: <API_KEY>
    Authorization: Bearer <API_KEY>

**A customer token** identifies a single store customer and grants access to that customer's own data
only. Use it for storefronts, mobile apps and anything acting on behalf of a shopper.

    Authorization: Bearer <CUSTOMER_TOKEN>

The Authorization header wins when both are present. A missing or wrong credential is refused with 401.
There is no session or cookie authentication: being signed in to the admin area does not authenticate an
API request.

## Getting the API key

Exchange administrator credentials for it. Only a member of the Administrators role is served.

    POST /api/rest/token
    { "email": "admin@example.com", "password": "..." }

## Getting a customer token

Exchange store credentials for it. Registered customers only; guests and administrators are refused.

    POST /api/rest/customer/token
    { "email": "customer@example.com", "password": "..." }

Returns the token, the customer identifier it belongs to, and how many seconds it stays valid. Tokens
last 1 hour and are signed with the plugin API key, so **regenerating the API key invalidates every
token already issued**.

Both token endpoints are exempt from the credential check, because they are how a credential is obtained
in the first place.

In the Swagger UI click Authorize and paste the key. The admin cookie is not used, and the customer
scoped operations expect a customer token rather than the API key.

## Authorization rules

| Route | Credential |
|---|---|
| `/api/rest/token`, `/api/rest/customer/token` | none |
| `/api/rest/customer/me/*` | customer token; the API key alone gets 403 |
| writes (POST/PUT/PATCH/DELETE) | API key or customer token |
| reads | API key or customer token when "Require API key for reads" is on, otherwise anonymous |

The `/api/rest/customer/me/*` routes always need a credential, whatever the read setting says: they
expose one customer's addresses, orders and wishlist, and leaving them open would hand every anonymous
caller the data of any customer who can supply an identifier.

Every `me` route takes the customer from the credential itself. There is no customer parameter anywhere
in their query strings, paths or bodies, so a caller cannot reach another customer's data by changing a
value.

## Configuration

Settings live on the plugin configuration page and are stored per store, so each tenant can hold its own
key.

- API key: generated on the configuration page, or written there directly. Must be at least 32 characters,
  because it also signs customer tokens. Use the generate button rather than typing a short key.
- Require API key for reads: when enabled, GET requests need a credential too.
- Rate limit per minute: requests allowed per client per minute, default 60.

Note that leaving the read setting disabled also exposes customer and order reads, which contain personal
data. Enable it for any store reachable from outside a trusted network.

The Swagger document mirrors these rules per operation, and the customer scoped operations are published
with the customer token scheme, because they refuse the API key. `POST /api/rest/token` reports the
guarded methods in its `SecuredMethods` field.

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
- `POST /api/rest/token` - administrator credentials, returns the API key
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
- `GET /api/rest/store/products/{id}` - the storefront view of a product, with its variants, manufacturers and tags. Unpublished or deleted products return 404, so the storefront cannot tell the difference.

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
Set the `base_url` and `api_key` variables before running.

## Notes

- API-key and rate-limit middleware applies to requests under /api/rest only. The plugin Swagger UI is excluded.
- Rate limited clients are bucketed per validated credential, hashed: the admin key gets one bucket, each customer token holder gets their own. Requests without a valid credential are bucketed per IP.
- Log records are readable at /api/rest/logs, and a failed record carries its full exception message. Enable the read setting to keep that behind the key.
- MFA cannot be used through the API: there is no way to complete an interactive challenge, so such accounts are refused at the token endpoints.
- Avoid returning domain entities directly; the plugin uses DTOs to define the public contract.
