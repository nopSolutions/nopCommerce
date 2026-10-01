# Plan: official-shaped token routes (`/api-backend`/`/api-frontend`)

**Status: REVERTED.** Everything this plan added has been removed. The two alias attributes are gone, as
are `ApiRestDefaults.BackendTokenRoute`/`FrontendTokenRoute`, the `CustomOperationIds` workaround that
existed only because an action sat at two paths, and the `IsTokenPath` clause in the middleware's
`UseWhen` predicate. Only `/api/rest/token` and `/api/rest/customer/token` are served now.

Kept as a record of *why* the aliases existed — clients written against the official API expect those
paths — and why they went: they were the only aliased pair among fifteen controllers, so they were the
only place a consumer had to make a choice that could not matter. The cost of official-API path
compatibility was judged higher than the benefit.

The two Swagger documents named `api-backend` and `api-frontend` are unrelated and still stand; see
[`backend-frontend-split.md`](backend-frontend-split.md). Those are document names, not route aliases.

**Original plan below, for the record.**

One change differs from the plan below, and one was added. Item 4 was dropped in
favour of a second attribute route on the existing actions (`[HttpPost("/api-backend/Authenticate/
GetToken")]` next to the existing `[HttpPost]`), because `Infrastructure/RouteProvider.cs` already exists
and exists precisely to call `MapControllers()`, so a conventional route would have needed a duplicate of
the endpoint rather than an extra path onto the action already serving it. Attribute routing also means
ApiExplorer and Swagger publish the official paths, which a conventional route would not. The extra
change: `GetTokenRequest.Email` lost `[Required]`, because the official API's clients send `username`
alone on stores with usernames enabled and the old annotation rejected those requests before either
action could pick the right identifier. Both actions already reject a request carrying neither.

## Goal
Expose the official nopCommerce Web API token shapes so the supplied curl works against the local
plugin, with **identical behavior** to the existing `/api/rest/token` and `/api/rest/customer/token`:

- `POST /api-backend/Authenticate/GetToken` → admin credentials → returns the shared API key (reused
  as a bearer token, per README). Maps to the existing `ApiKeyController.Token` logic.
- `POST /api-frontend/Authenticate/GetToken` → store credentials → returns a 7-day customer JWT.
  Maps to the existing `CustomerTokenController.Token` logic.

Accepts body `{ username, email, password }` (already supported by `GetTokenRequest`). No cookies —
the plugin's own credential model applies. `application/json-patch+json` binds via the host's
Newtonsoft JSON formatter (already registered).

## Design decision
Mirror the host's fixed-literal conventional route pattern (e.g. `robots.txt` → `Common.RobotsTextFile`)
instead of touching the working attribute routes. This reuses the **exact** existing actions (no
duplicated logic) and leaves `/api/rest/*` untouched (zero regression risk).

## Changes

1. **`ApiRestDefaults.cs`** — add `BackendTokenRoute` / `FrontendTokenRoute`; extend `IsTokenPath`
   to recognise them (so they are credential-exempt and rate-limited, not guarded by the API key).
   `IsApiPath` is left as-is on purpose.

2. **`Infrastructure/NopStartup.cs`** — extend the `UseWhen` predicate so the rate-limit/credential
   middleware branch is also entered for the official token routes:
   `StartsWithSegments("/api/rest") || ApiRestDefaults.IsTokenPath(...)`

3. **`Infrastructure/ApiKeyRateLimitMiddleware.cs`** — widen the gate so official token routes are
   not short-circuited: `if (!IsApiPath(path) && !IsTokenPath(path)) { await _next; return; }`.
   Token paths remain credential-exempt and are still rate-limited (brute-force protection).

4. **`Infrastructure/RouteProvider.cs`** — add two conventional routes (literal patterns + defaults)
   mapping the official paths to the existing `ApiKey`/`CustomerToken` `Token` actions:
   `api-backend/Authenticate/GetToken` → `ApiKeyController.Token`
   `api-frontend/Authenticate/GetToken` → `CustomerTokenController.Token`

5. **Docs** — `README.md` (Endpoints + "Getting the API key / customer token"), `API-COVERAGE.md`
   (note the official shapes are now also served directly).

## Verification
- `dotnet build src\Plugins\Nop.Plugin.Api.Rest\Nop.Plugin.Api.Rest.csproj` compiles.
- No changes to the `/api/rest/*` behavior (gate/UseWhen only ADD coverage). `IsApiPath` unchanged.
- Swagger still marks token endpoints as unsecured; the new routes appear under the plugin assembly.

## Out of scope
- Phase 4 catalog/customer write endpoints (not requested).
- Official routes for non-token operations (none mirrored yet).
