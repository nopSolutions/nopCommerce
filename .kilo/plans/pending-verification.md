# Pending verification — Nop.Plugin.Api.Rest

**Status: built, deployed, and largely live-checked. Four checks still need a registered shopper
account, and one fix shipped at 16:55:57 is unverified. Nothing is committed and the working tree is
partially staged.**

Written at the end of the session that built the backend/public-store split. The design decisions and
their reasoning live in the three sibling plan documents; this file is only about what is **proven**,
what is **not**, and how to check the rest.

---

## 1. What is deployed

| Capability | Detail |
|---|---|
| One credential per header | `Authorization: Bearer` carries a signed JWT; `X-Api-Key` carries the raw key. Neither is accepted in the other's place. |
| Two token endpoints | `POST /api/rest/token` (admin) and `POST /api/rest/customer/token` (customer). Both return the same JWT shape; the `nop:credential` claim records the scope. |
| Two Swagger documents | `swagger/api-backend/swagger.json` and `swagger/api-frontend/swagger.json`, chosen from Swagger's "Select a definition" dropdown on one page at `/swagger/api-rest/index.html`. |
| Scope enforced both ways | A customer token is refused on back-office routes; an admin-level credential is refused on `/api/rest/customer/me/*` and `/api/rest/store/*`. |
| Configurable lifetimes | `AdminTokenLifetimeHours` (24) and `CustomerTokenLifetimeDays` (7), per store. |
| Store scoping | `GET /api/rest/store/products/{id}` honours an optional `X-Store-Id` header. |
| No route aliases | Only `/api/rest/token` and `/api/rest/customer/token` are served. The `/api-backend/…` and `/api-frontend/…` spellings were added and then reverted; see `add-official-token-routes.md`. |

---

## 2. Bugs found and fixed

All eight were found by asking "why does this behave like that?" rather than by reading the code
top-down. Three of them were the same failure: **the published document disagreeing with the runtime.**

| Symptom | Root cause | File |
|---|---|---|
| `GET /api/rest/customer/me/*` returned an empty 404 for every caller | The middleware validated the token against its own scheme and then never published the principal, so the controller read `HttpContext.User`, which the host had populated from its **cookie** scheme. No `CustomerId` claim there, so it resolved to nobody. | `Infrastructure/ApiKeyRateLimitMiddleware.cs` |
| A customer token could rewrite the catalog and cancel other people's orders | `HasCustomerCredential` was consulted in exactly one place. Every other route asked only "was any credential presented?", so the `nop:credential` claim was a label nothing read. Both token kinds are signed with the same key. | `Infrastructure/ApiKeyRateLimitMiddleware.cs` |
| `GET /api/rest/store/products/{id}` served products from other stores' catalogues | It filtered only on `Published` and `Deleted`, which are **store-agnostic** product flags, and took no store parameter at all. | `Controllers/StoreProductsController.cs` |
| The plugin configuration page **never saved anything** | `<Nullable>annotations</Nullable>` in the csproj enables the annotations context, and `[ApiController]` turns every non-nullable reference-type property into an implicit `[Required]`. Four display-only properties are populated on GET and never posted, so every POST failed validation — and `if (!ModelState.IsValid) return await Configure();` re-rendered the page with no notification and no validation summary. | `Models/ConfigurationModel.cs`, `Controllers/ApiRestController.cs`, `Views/Configure.cshtml` |
| Both token endpoints returned 400 for any body omitting `email` **or** `username` | Same implicit `[Required]`, this time on `GetTokenRequest`. It silently defeated the earlier removal of the explicit `[Required]` from `Email` and contradicted the README and the model's own documentation. A store with usernames enabled sends `username` alone. | `Models/Requests/GetTokenRequest.cs` |
| Swagger showed no lock on secured reads, and the generated curl carried no `X-Api-Key` | `ApiKeySecurityOperationFilter` took `IServiceProvider` in its constructor — the **root** provider — while `ApiRestSettings` is registered `AddScoped`. The filter built the document from one app-lifetime settings instance; the middleware resolved the same settings per request. The runtime was right; the document was stale. | `Infrastructure/ApiKeySecurityOperationFilter.cs` |
| The `ApiKey` scheme appeared in the public store document, which 403s it | `AddSecurityDefinition` is document-agnostic, so both credentials landed on both documents and Swagger UI listed them all in the Authorize dialog. | `Infrastructure/FrontendSecuritySchemeFilter.cs` (new) |
| The storefront projection showed a lock icon even when anonymous callers were served it | The filter returned `Bearer` for `IsFrontendPath` *before* consulting the read setting. The middleware's rule is `IsCustomerScopePath(path) || (IsStorefrontScopePath(path) && readsProtected)`. | `Infrastructure/ApiKeySecurityOperationFilter.cs` |

A note on the wrong turn worth not repeating: the stale-page `[Range(1, int.MaxValue)]` theory for the
silent save was wrong. The nullable context was the cause. The `[Range]` fields *do* still refuse a
genuinely stale browser page — now visibly, naming the field.

---

## 3. Verified live

All results below were produced against the running site with
`apirestsettings.requireapikeyforreads = True`.

### Runtime

| Request | Result | Meaning |
|---|---|---|
| `GET /api/rest/categories` anonymous | 401 | read protection active |
| `GET /api/rest/categories` + `X-Api-Key` | 200 | |
| `GET /api/rest/customers` + `X-Api-Key` | 200 | |
| `GET /api/rest/categories` raw key as `Authorization: Bearer` | 401 | slots are not cross-accepted |
| `GET /api/rest/store/products/1` anonymous | 403 | storefront needs a customer token once reads are protected |
| `GET /api/rest/store/products/1` + `X-Api-Key` | 403 | admin credential refused on the public store |
| `GET /api/rest/customer/me` anonymous | 403 | always needs a credential |
| `GET /api/rest/customer/me` + `X-Api-Key` | 403 | admin credential refused |
| `X-Api-Key` with a trailing space | 200 | trimmed before comparison |
| `POST /api/rest/token` bad password | 401 | reachable without a credential |
| `POST /api/rest/customer/token` bad password | 401 | reachable without a credential |
| `POST /api/rest/customer/token` **username only, no email** | 400 `{"error":"Both 'email' and 'password' are required."}` | **that is the action's own message, not `ValidationProblemDetails`** — so the body bound and reached the action. Before the `string?` fix this returned `"username":["The username field is required."]` |

### Published documents

| | Backend | Public store |
|---|---|---|
| `securitySchemes` | `ApiKey`, `Bearer` | **`Bearer` only**; the raw JSON contains no `ApiKey` reference at all |
| `/api/rest/categories` | `ApiKey or Bearer` | absent |
| `/api/rest/customers`, `/api/rest/orders` | `ApiKey or Bearer` | absent |
| `/api/rest/token` | unsecured | absent |
| `/api/rest/customer/token` | absent | unsecured |
| `/api/rest/customer/me`, `/customer/me/addresses` | absent | `Bearer` |
| `/api/rest/store/products/{id}` | absent | `Bearer` (setting ON) |

Both documents parse as valid JSON — no dangling `$ref`, which matters because deleting a declared
scheme that an operation still references is invalid OpenAPI and makes generators reject the document.

### Database

```text
apirestsettings.admintokenlifetimehours   [24]
apirestsettings.apikey                    [3CcG5kLbiSjx0KsT8NUTSYGDcb0IAgTEGxiR1kj1qvA=]
apirestsettings.customertokenlifetimedays [7]
apirestsettings.ratelimitperminute        [60]
apirestsettings.requireapikeyforreads     [True]
```

The API key is 44 characters: base64 of the 32 bytes `IEncryptionService.CreateSaltKey(32)` produces.
It is stored verbatim, **not** encrypted — it merely looks encoded.

---

## 4. Outstanding

### Needs a registered shopper account

Cannot be produced without resetting a password on real customer data, so these were not attempted:

1. Customer token → all five `GET /api/rest/customer/me/*` routes → **200**. *This is the original 404 bug.*
2. Customer token → `POST /api/rest/products` → **403**. *This is the escalation.*
3. Admin token → `GET /api/rest/customer/me` → **403**.
4. `GET /api/rest/store/products/{id}` with `X-Store-Id` naming a store the product is not mapped to → **404**.

### Shipped at 16:55:57, not yet verified

The storefront lock-icon fix (`ApiKeySecurityOperationFilter` now honours the read setting for
storefront paths). To prove both halves, untick *Require a credential for reads*, Save, and confirm:

| Check | Expected |
|---|---|
| `GET /api/rest/categories` anonymous | 200 |
| `GET /api/rest/store/products/{id}` anonymous | 200 |
| `GET /api/rest/customer/me` anonymous | still 403 |
| `/swagger/api-frontend/swagger.json` → `/api/rest/store/products/{id}` | **(unsecured)** |
| same document → `/api/rest/customer/me` | still **`Bearer`** |

### Needs a full plugin reinstall to re-verify

`InstallAsync` now writes the five setting defaults and `UninstallAsync` deletes them. That seeding has
run once on this database (see §3) but has not been exercised end-to-end since.

---

## 5. How to verify

### Build

```powershell
dotnet build src\Plugins\Nop.Plugin.Api.Rest\Nop.Plugin.Api.Rest.csproj `
  -p:SolutionDir="D:\PROJECTS\nopCommerce\src\" -v q --nologo
```

`SolutionDir` is mandatory. The project reference is `$(SolutionDir)\Presentation\Nop.Web\Nop.Web.csproj`
and the property is undefined when the csproj is built directly, producing roughly 238 phantom
`CS0246: type or namespace not found` errors for `Nop.Services.*` and `Nop.Core.Domain.*`. Building via
`src\NopCommerce.sln` works too.

### Deploy

**Stop the Visual Studio debugger first.** A running `Nop.Web` holds
`src\Presentation\Nop.Web\Plugins\Api.Rest\Nop.Plugin.Api.Rest.dll` open and the post-build copy fails
with `MSB3026` / `MSB3021` after 10 retries. Compilation still succeeds, so a "0 Errors" build says
nothing about whether the deploy landed — check the DLL's `LastWriteTime`.

### Database

```powershell
docker exec nopcommerce_mysql_server mysql -uroot -pnopCommerce_db_password -D nopcommerce_database `
  -e "SELECT Name,Value FROM Setting WHERE Name LIKE 'apirestsettings%';"
```

Container names come from `docker ps`. Credentials are in
`src\Presentation\Nop.Web\App_Data\appsettings.json`.

### Posting JSON from PowerShell

**`curl -d '{"email":"a@b.com"}'` does not work.** PowerShell strips the embedded double quotes, the
JSON fails to parse, and the result is a misleading 400 that looks like a model-validation failure.
Write the body to a file and reference it:

```powershell
$t = "C:\Users\USER\AppData\Local\Temp\opencode"
Set-Content -LiteralPath "$t\body.json" -Value '{"username":"someone","password":"bad"}' -NoNewline
curl.exe -s -X POST "http://localhost:54720/api/rest/customer/token" `
  -H "Content-Type: application/json" -d "@$t\body.json"
```

### Reading a document

```powershell
$sw = curl.exe -s "http://localhost:54720/swagger/api-backend/swagger.json" | ConvertFrom-Json
$sw.paths.'/api/rest/categories'.get.security | ForEach-Object { $_.PSObject.Properties.Name }
```

`$null` there means the operation is published as unsecured.

---

## 6. Open items

1. **Open decision — this session introduced it.** `UninstallAsync` now calls
   `DeleteSettingAsync<ApiRestSettings>()`, and `InstallAsync` seeds `ApiKey = string.Empty`. So
   uninstalling and reinstalling the plugin **silently discards a generated API key**; every token signed
   with it dies too. That follows the standard nopCommerce pattern of deleting plugin settings on
   uninstall, but destroying a merchant's credential without warning is hostile, and it is why the key
   in §3 is not the one from before this session. Worth deciding whether to preserve the key across a
   reinstall. The alternative — leaving the rows behind — is untidy but not surprising.
2. **`UseAuthorization` runs at order 600, this middleware at 700.** So `[Authorize]` on a plugin API
   controller would be evaluated against the cookie identity rather than the API credential. Nothing
   under `/api/rest` uses `[Authorize]`; only the configuration page does. Left alone as a pre-existing
   ordering gap.
3. **`IsRegisteredAsync` only checks the `Registered` role** (`CustomerService.cs:1358`), so an
   Administrators account that is also in `Registered` can still obtain a customer token. The comment on
   that controller claims administrators are refused, which the check does not actually guarantee.
4. **`$(SolutionDir)` in the csproj** — see §5. Fixing it would let the plugin build standalone.

---

## 7. Git state

**Nothing has been committed.** The tree is partially staged, most likely from `git mv` plus an
explicit `git add` elsewhere:

- Staged (`M` in the first column): most of `src/Plugins/Nop.Plugin.Api.Rest`.
- `ApiKeySecurityOperationFilter.cs` is `MM` — staged, then modified again afterwards.
- `.kilo/plans/add-official-token-routes.md` is modified but unstaged.
- `.kilo/plans/backend-frontend-split.md` and `.kilo/plans/one-token-format-per-slot.md` are **untracked**.
- `mysql-docker-compose.yml` and `postgresql-docker-compose.yml` are modified and were **not** touched by
  this session's work.

Deciding what to stage and commit is the user's call, not the next agent's.