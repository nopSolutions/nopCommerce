# Plan: split the API into a backend and a public store document

**See also [`pending-verification.md`](pending-verification.md)** for what is actually proven at runtime,
what is still unverified, and the environment gotchas. This file records design decisions; that one
records evidence.

**Status: implemented.** The split was asked for as a documentation change — two Swagger pages like the
official demo — and turned out to require an enforcement change underneath it, because the plugin had no
backend/frontend boundary at all. Both halves are below.

## The bug this surfaced

`GET /api/rest/customer/me` was fixed by
[`one-token-format-per-slot.md`](one-token-format-per-slot.md). While checking how the two token scopes
differed, it became clear they did not:

- `HasCustomerCredential` was consulted in exactly one place — the `/customer/me/*` gate.
- Every other route asked only `!isAuthenticated.Succeeded`, i.e. *was any credential presented*.

So a **customer token had full admin write access**. A registered shopper could post their own username
and password to `POST /api/rest/customer/token` and then `POST /api/rest/products`,
`PATCH /api/rest/products/{id}` or `POST /api/rest/orders/{id}/cancel` on other people's orders. Both
token kinds are signed with the same shared API key, so `nop:credential` was a label nothing read.

Pre-existing, not a regression from the previous plan: the old code had the identical check and its
README said outright that writes accept "API key or customer token".

## Why one predicate

`ApiRestDefaults.IsFrontendPath` is the single source of the split, consumed by three places that would
otherwise drift:

1. **`ApiKeyRateLimitMiddleware`** — refuses a customer token on `IsAdminApiPath`, and an admin level
   credential on `IsFrontendPath`. This is the enforcement.
2. **`DocInclusionPredicate`** — routes each operation into the backend or frontend document.
3. **`ApiKeySecurityOperationFilter`** — advertises `Bearer` alone on frontend operations and
   `X-Api-Key` OR `Bearer` on guarded backend ones.

Splitting the documents alone would have changed nothing about what the API accepts.

## Classification

| Path | Side |
|---|---|
| `api/rest/token` | backend |
| `api/rest/customer/token` | frontend |
| `api/rest/customer/me/*` | frontend |
| `api/rest/store/*` | frontend |
| everything else under `api/rest` | backend |

Token endpoints are on neither side, because both documents publish their own and neither credential
admits the other.

## Store selection on the storefront projection

`GET /api/rest/store/products/{id}` filtered only on `Published` and `Deleted`, both **store-agnostic**
product flags. On a multi-store install it returned products belonging to a different store's catalog.

It now accepts an `X-Store-Id` header and passes it to `IStoreMappingService.AuthorizeAsync`, which is
the primitive nopCommerce already provides and which honours `LimitedToStores`, `IgnoreStoreLimitations`
and `storeId == 0`. The header can therefore only **narrow** a response, never widen one, and omitting it
degrades to the previous unfiltered behaviour so clients predating the header keep working.

Note the official API divides the two suites differently here: `api-backend` takes `storeId` as an
explicit parameter, while `api-frontend` has the client name its store on a header. This plugin's
storefront projection follows the latter.

## Changes

1. **`ApiRestDefaults.cs`** — `StorefrontScopeRoutePrefix`, `IsStorefrontScopePath`, `StoreIdHeaderName`,
   `IsAdminTokenPath`/`IsCustomerTokenPath` (splitting `IsTokenPath`, which is now their union),
   `IsFrontendPath`, `IsAdminApiPath`. `SwaggerJsonPath` becomes `SwaggerBackendJsonPath` and
   `SwaggerFrontendJsonPath`.

2. **`ApiKeyRateLimitMiddleware.cs`** — the two-directional scope gate, replacing the
   customer-scope-only check. Token paths are exempt so a credential can still be obtained.

   The gate distinguishes three things rather than two, which the first draft got wrong twice:
   `/customer/me/*` needs a customer token unconditionally; `/store/*` needs one only once reads are
   protected, because it is what an anonymous visitor browses; and **both** public store paths refuse a
   *presented* admin credential while letting an anonymous caller fall through to the read rule. Treating
   `/store/*` as "always needs a customer token" would have broken anonymous storefront browsing, and
   treating it as "no scope rule" would have left the split half-applied.

3. **`StoreProductsController.cs`** — injects `IStoreMappingService`, reads the optional header, 404s a
   product not mapped to the requested store.

4. **`Infrastructure/NopStartup.cs`** — two `SwaggerDoc` groups and two `SwaggerEndpoint` registrations.
   Registering the endpoint twice is what makes Swagger UI render the "Select a definition" dropdown, so
   there is one page rather than two. Each document carries its own `OpenApiInfo.Description`, because the
   two are read by different audiences and sharing one block would have to describe both.

5. **`ApiKeySecurityOperationFilter.cs`** — requirement derived from `IsFrontendPath`.

6. **`Infrastructure/FrontendSecuritySchemeFilter.cs`** — `AddSecurityDefinition` cannot be scoped to
   one document, so both credentials landed on both and the public store document advertised an API key
   its own operations refuse with 403. This `IDocumentFilter` drops that scheme from the public store
   document, sweeps any requirement still naming it (a dangling `$ref` is invalid OpenAPI and makes
   generators reject the document), and replaces the shared `Bearer` description, which had been written
   for the back office and mentioned "the API key above" where there is none. The definitions declared in
   `NopStartup` are now the back office ones only.

7. **Config page** — `SwaggerJsonUrl` replaced by `SwaggerBackendJsonUrl` and `SwaggerFrontendJsonUrl`
   across `ConfigurationModel`, `ApiRestController`, `Configure.cshtml`, and the `Plugin.cs` locale
   strings.

## Verification
- `POST /api/rest/products` with a **customer** token → **403**. This is the escalation, closed.
- `GET /api/rest/store/products/{id}` with a customer token → 200; with an admin token → 403.
- `GET /api/rest/customer/me` with an admin token → 403, unchanged.
- `X-Api-Key` and admin token on back office routes → unchanged.
- `GET /api/rest/store/products/{id}` with `X-Store-Id` naming a store the product is not mapped to → 404.
- `/swagger/api-rest/index.html` shows the dropdown; each JSON contains only its own operations.
- `/swagger/api-frontend/swagger.json` → `securitySchemes` holds `Bearer` only, with no `$ref` to
  `ApiKey` anywhere in the document, and opens as valid OpenAPI.
- `/swagger/api-backend/swagger.json` → both schemes, unchanged.
- Build: `dotnet build src\Plugins\Nop.Plugin.Api.Rest\Nop.Plugin.Api.Rest.csproj -p:SolutionDir=...`, or
  through `src\NopCommerce.sln`. Building the csproj directly without `SolutionDir` produces ~238 phantom
  "type not found" errors.

## Breaking changes (three, cumulative)
1. `POST /api/rest/token` returns a token instead of `{ apiKey }`, and the raw key is no longer accepted
   as a bearer token.
2. A customer token is refused on back office routes, and an admin level credential is refused on
   `/api/rest/store/*`.
3. `/api-backend/Authenticate/GetToken` and `/api-frontend/Authenticate/GetToken` are gone; only
   `/api/rest/token` and `/api/rest/customer/token` are served. See
   [`add-official-token-routes.md`](add-official-token-routes.md) for why the aliases were reverted.

Customer tokens issued before this change keep validating: they already carried `nop:credential` and
`CustomerId`.

## Later: the route aliases were reverted

Removing the official path spellings turned out to simplify three things that existed only to support
them: the second `[HttpPost]` attribute on each token action, the `CustomOperationIds` override (needed
because one action at two paths emitted a duplicate `operationId`, which is invalid OpenAPI), and the
`IsTokenPath` clause in the middleware's `UseWhen` predicate (redundant once both token routes sat under
`/api/rest`). Operation ids went back to Swashbuckle's `Controller_Action` default.

## Found while verifying: the configuration page had never saved

Ticking "Require a credential for reads", pressing Save and still finding `GET /api/rest/categories`
anonymous is **not by design**. The `Setting` table held 759 rows and exactly one for this plugin,
`apirestsettings.apikey`. No `requireapikeyforreads` row existed, so `LoadSettingAsync` returned the C#
default of `false` and reads were anonymous — correct behaviour for `false`, wrong for what was on
screen.

**The cause was the nullable annotations context, not the range attributes.** `Nop.Plugin.Api.Rest.csproj`
sets `<Nullable>annotations</Nullable>`, which turns on the annotations context, and ASP.NET Core's
`[ApiController]` treats every non-nullable reference type property as an implicit `[Required]`. The host
never sets `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes`. So every display-only property
on `ConfigurationModel` — `SwaggerUiUrl`, `SwaggerBackendJsonUrl`, `SwaggerFrontendJsonUrl` and
`GeneratedApiKey` — failed validation on every POST, `ModelState.IsValid` was false, and
`if (!ModelState.IsValid) return await Configure();` re-rendered the page **without a notification and
without a validation summary**. An admin saw a page that looked saved. That is also why the only surviving
row was the API key: `GenerateApiKey` is a separate action that never binds `ConfigurationModel`.

Three fixes:

1. **The display-only properties are now `string?`.** `ApiKey` too, since blank already means "keep the
   current key". Every other request model in the plugin already used `string?`; `ConfigurationModel` and
   `GetTokenRequest` were the only two that had not, and `GetTokenRequest` was the same latent bug on a
   live endpoint: a bare `public string Email` meant both token endpoints returned **400 for any request
   omitting `email` or `username`**, which is exactly what a store with usernames enabled sends. That
   silently defeated the earlier removal of `[Required]` from `Email` and contradicted the README and the
   model's own documentation. Both are now `string?`.

2. **The form carries a validation summary** and the action raises an error notification, so a refused
   save is visible rather than indistinguishable from a successful one.

3. **`InstallAsync` writes every setting default** and `UninstallAsync` deletes them. The rows did not
   exist at all, so the effective values were C# defaults rather than anything stored, and a missing
   `requireapikeyforreads` row failed **open**, exposing the back office customer and order reads by
   default.

A note on the wrong turn: `[Range(1, int.MaxValue)]` on the two token lifetime fields does make a page
rendered before those fields existed fail on `0`. That was the first suspect and it was not the cause,
though it will still refuse a stale browser page — now visibly, naming the field.

## Left alone
- `UseAuthorization` (order 600) still runs before this middleware (700), so `[Authorize]` on a plugin
  API controller would evaluate the cookie identity. Nothing under `/api/rest` uses `[Authorize]`.
- `CustomerTokenController` refuses non-customers with `IsRegisteredAsync`, which only checks the
  `Registered` role (`CustomerService.cs:1358`), so an Administrators account that is also in `Registered`
  still gets a token.