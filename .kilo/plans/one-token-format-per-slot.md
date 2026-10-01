# Plan: one token format per credential slot

**Followed by [`backend-frontend-split.md`](backend-frontend-split.md)**, which enforces the two token
scopes against each other and publishes the API as two Swagger documents. Everything below still holds.

**Status: implemented.** The bug this plan was written to fix is the reason the work was needed:
`GET /api/rest/customer/me` returned an empty 404 for every caller, because the middleware validated the
bearer token against its own scheme and then never published that principal, so the controller read
`HttpContext.User` — which the host had already populated from its *cookie* scheme — and found no
`CustomerId` claim in it. Fixing only that would have left three overlapping ways to authenticate, so
the credential model was simplified at the same time.

## Goal

Make `Authorization: Bearer` carry exactly one kind of value, and keep the shared API key valid in
`X-Api-Key` only:

| Credential | Header | Format |
|---|---|---|
| Bearer token (admin or customer scope) | `Authorization: Bearer <jwt>` | signed JWT |
| Shared API key | `X-Api-Key: <key>` | opaque key |

Before this change `Authorization: Bearer` accepted both the raw API key *and* a JWT, and Swagger
published three overlapping security schemes. A caller holding both credentials could present the wrong
one and be silently given the scope of the other.

## Why the scope has to stay in the token

The two endpoints stay separate — admin credentials go to `/api/rest/token`, store credentials to
`/api/rest/customer/token` — because they authorize different things, and the official API's own division
between its back office and public store suites maps onto them one to one. What changed is that both now
return the *same* token format, with the `nop:credential` claim recording the scope. `HasCustomerCredential`
reads that claim, so an admin token is refused by `/api/rest/customer/me/*` and a customer token is
refused by nothing it should not be.

## Changes

1. **`ApiRestDefaults.cs`** — `GetTokenFromRequest` (which preferred Bearer over `X-Api-Key` and returned
   a bare string) is replaced by `GetCredential`, returning a `(CredentialSlot, Value)` pair. The slot has
   to travel with the value because the two slots accept different formats and neither is read as the
   other. Static `CustomerTokenLifetime` removed; `SecuritySchemeId` renamed `ApiKeySchemeId`;
   `CustomerTokenSchemeId` removed.

2. **`Security/CustomerTokenFactory.cs` → `Security/ApiTokenFactory.cs`** — generalised to both scopes.
   `CreateToken` takes a credential type and omits the `CustomerId` claim unless it is a customer token;
   `TryValidateToken` reports the scope it read and rejects a token that names none.

3. **`Security/ApiRestApiKeyAuthenticationHandler.cs`** — validates strictly by slot: `X-Api-Key` by
   constant-time comparison only, `Bearer` by signature only. The cross-accept branch is gone.

4. **`Infrastructure/ApiKeyRateLimitMiddleware.cs`** — two fixes:
   - publishes the validated principal onto `HttpContext.User`, which is what the 404 was. Safe because
     this middleware runs at order 700, between `UseAuthorization` (600) and `UseEndpoints` (900), so
     nothing downstream re-authenticates, and only `/api/rest*` paths reach it, so the admin area keeps
     authenticating by cookie.
   - `BuildClientId` buckets by scope rather than by hashing the presented value. Hashing would have
     given every admin token its own bucket — they are unique per issuance — and the limit would silently
     never trigger. Customers bucket by `customer:{id}`, everything admin level shares one bucket.
   `HashClientKey` is removed as dead code.

5. **`Controllers/ApiKeyController.cs`** — returns a token instead of `ApiKeyDto`. `ApiKeyDto` is
   deleted; `CustomerTokenDto` becomes `ApiTokenDto`, one shape for both scopes, with the customer fields
   nullable for an admin token.

6. **`ApiRestSettings` / `ConfigurationModel` / `ApiRestController` / `Views/Configure.cshtml` /
   `Plugin.cs`** — `AdminTokenLifetimeHours` (24) and `CustomerTokenLifetimeDays` (7) become settings,
   wired through the existing per-store override pattern.

7. **`Infrastructure/NopStartup.cs` / `ApiKeySecurityOperationFilter.cs`** — three security definitions
   down to two. Customer scoped operations advertise `Bearer`, guarded operations advertise
   `X-Api-Key` OR `Bearer`.

8. **Copy only: `OpenApiInfo.Description` and the two security scheme descriptions.** The info block is
   now the credential guide — what each credential is, where to get it, what it grants, the 403 rules
   between scopes, the cookie rule, and the consequence of regenerating the key. The scheme descriptions
   stayed short pointers, because the Authorize dialog that renders them is too narrow to carry it. Lists
   are used rather than markdown tables, so the content does not depend on Swagger UI's table support.

## Verification
- `dotnet build src\Plugins\Nop.Plugin.Api.Rest\Nop.Plugin.Api.Rest.csproj` compiles. The project
  references `$(SolutionDir)`, so building the csproj directly fails to resolve the host projects unless
  that property is supplied or the build goes through `src\NopCommerce.sln`.
- Admin token on `/api/rest/customer/me` → 403. Customer token on all five `/api/rest/customer/me/*`
  routes → 200. This is the case that used to 404.
- Raw key in `X-Api-Key` → accepted. Raw key in `Authorization: Bearer` → 401.
- 70 admin requests in a minute → 429 on one shared bucket.
- `/swagger/v1/swagger.json` → two security definitions, no duplicate operationIds.
- Admin area pages still authenticate by cookie.

## Breaking change
`POST /api/rest/token` no longer returns `{ apiKey }`, and the raw API key is no longer accepted as a
bearer token. Anything following "paste the returned key into Authorize" must paste the token instead.
Customer tokens issued before this change keep validating: they already carried `nop:credential` and
`CustomerId`.

## Out of scope
- `UseAuthorization` (order 600) runs before this middleware (700), so `[Authorize]` on a plugin API
  controller would evaluate the cookie identity rather than the API credential. Nothing under `/api/rest`
  uses `[Authorize]` today; only the configuration page does.
- `CustomerTokenController` refuses non-customers with `IsRegisteredAsync`, which only checks for the
  `Registered` role (`CustomerService.cs:1358`). An Administrators account that is also in `Registered`
  therefore gets a token. Pre-existing, and the admin endpoint guards on `IsAdminAsync` instead.