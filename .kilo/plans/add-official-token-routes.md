# Plan: official-shaped token routes (`/api-backend`/`/api-frontend`)

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
