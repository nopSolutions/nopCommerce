using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Nop.Plugin.Api.Rest.Infrastructure
{
    internal class RateLimitEntry
    {
        public int Count { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }

    /// <summary>
        /// Applies the per client rate limit and enforces which scope of the API a credential may reach
        /// </summary>
        /// <remarks>
        /// The credential itself is validated by <see cref="Security.ApiRestApiKeyAuthenticationHandler"/>,
        /// so that each slot is checked only as what it is: the API key by a constant time comparison,
        /// and the bearer token by its signature. This middleware decides what that credential is allowed
        /// to do, from the one classification in <c>ApiRestDefaults.IsFrontendPath</c>.
        /// </remarks>
    public class ApiKeyRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public ApiKeyRateLimitMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only protect plugin API routes. The official shaped token routes are protected too, because they are
            // where a credential is obtained and therefore the only endpoint worth brute forcing. The
            // swagger UI and its JSON document are served from root level paths, so they never reach this
            // middleware and need no exemption here.
            var path = context.Request.Path.Value ?? string.Empty;
            if (!ApiRestDefaults.IsApiPath(path) && !ApiRestDefaults.IsTokenPath(path))
            {
                await _next(context);
                return;
            }

            var isAuthenticated = await context.AuthenticateAsync(ApiRestDefaults.AuthenticationSchemeName);
            var isTokenPath = ApiRestDefaults.IsTokenPath(path);
            var isCustomerCredential = HasCustomerCredential(isAuthenticated);
            var readsProtected = RequiresApiKeyForReads(context);
            var requiresApiKey = !isTokenPath
                && ApiRestDefaults.RequiresApiKey(context.Request.Method, readsProtected);

            // The two scopes are kept apart in both directions, mirroring the official nopCommerce Web
            // API where the back office and the public store are separate suites. Both halves matter:
            //
            //  - the back office endpoints change the catalog, the orders and the customers. Both token
            //    kinds are signed with the same shared API key, so without this a registered shopper
            //    could mint a token from their own password and then rewrite the catalog or cancel other
            //    people's orders. The scope in the token is otherwise only a label nothing reads.
            //  - the customer scoped endpoints expose one customer's addresses, orders and wishlist, so
            //    they refuse an admin level credential, and they need one whatever the read setting says:
            //    leaving them open would hand every anonymous caller the data of any customer.
            //
            // The storefront catalog projection is deliberately not in that second group. It is what an
            // anonymous visitor browses, so it follows the read setting like any other read, and becomes
            // customer-token only once reads are protected. It still refuses an admin level credential,
            // which is the direction that matters here.
            //
            // The token endpoints are skipped entirely: they are how a caller obtains a credential in
            // the first place, so enforcing one here would make them unreachable.
            if (!isTokenPath)
            {
                var needsCustomerToken = ApiRestDefaults.IsCustomerScopePath(path)
                    || (ApiRestDefaults.IsStorefrontScopePath(path) && readsProtected);

                if (isCustomerCredential && ApiRestDefaults.IsAdminApiPath(path))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "This operation acts on the back office, which needs an admin level credential, not a customer token. Use the API key or a token from POST /api/rest/token."
                    });
                    return;
                }

                if (needsCustomerToken && !isCustomerCredential)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "This endpoint acts on the authenticated customer's own data and needs a customer token, not an admin level credential. Obtain one from POST /api/rest/customer/token and send it as 'Authorization: Bearer <token>'."
                    });
                    return;
                }

                //an admin level credential is refused on the public store side, because a storefront
                //client has no business holding one and shipping the shared key to a browser or a phone
                //is how it ends up in the wild. An anonymous caller is not refused here: it falls through
                //to the read rule below, which is what lets a visitor browse while reads are unprotected
                //and turns them away with a 401 once they are not.
                if (ApiRestDefaults.IsStorefrontScopePath(path)
                    && isAuthenticated.Succeeded && !isCustomerCredential)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "This endpoint projects the catalog for a public store client, which needs a customer token. Use a token from POST /api/rest/customer/token."
                    });
                    return;
                }
            }

            // The credential this middleware validated is not the one HttpContext.User holds. The host's
            // UseAuthentication ran earlier and populated User from its own default scheme, which is the
            // admin cookie, and the actions read the customer from User. Without this the request would be
            // authorised as the customer its token names and then resolved as whoever the cookie belongs
            // to, or as nobody, which reads as a 404 on every customer scoped route. Only API paths reach
            // this line, so the admin area keeps authenticating by cookie exactly as before.
            if (isAuthenticated.Succeeded)
                context.User = isAuthenticated.Principal;

            // Rate limiting runs before the credential is enforced, so that a rejected caller cannot
            // spend attempts. Clients are bucketed by the scope they proved rather than by the credential
            // they sent: every administrator token is distinct, so hashing the presented value would hand
            // each request a fresh bucket and the limit would never trigger. A customer is bucketed by
            // the customer its token names, so a shopper's traffic never spends the admin budget. No
            // credential is held as a cache key.
            var clientId = isAuthenticated.Succeeded
                ? BuildClientId(isAuthenticated)
                : $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

            if (!await TryConsumeAsync(context, clientId))
                return;

            if (requiresApiKey && !isAuthenticated.Succeeded)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "API key missing or invalid. Send it in the 'X-Api-Key' header, or get a bearer token from POST /api/rest/token and send that as 'Authorization: Bearer <token>'."
                });
                return;
            }

            await _next(context);
        }

        /// <summary>
        /// Checks whether the request presented a customer token rather than an admin level credential
        /// </summary>
        /// <param name="result">The authentication result for the request</param>
        /// <returns>True when the caller proved which customer it is</returns>
        protected static bool HasCustomerCredential(AuthenticateResult result)
        {
            if (!result.Succeeded)
                return false;

            var credentialType = result.Principal?.FindFirst(ApiRestDefaults.CredentialTypeClaim)?.Value;

            return string.Equals(credentialType, ApiRestDefaults.CredentialTypeCustomerToken,
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Builds the rate limit bucket of an authenticated caller
        /// </summary>
        /// <param name="result">The authentication result for the request</param>
        /// <returns>The bucket identifier</returns>
        /// <remarks>
        /// A customer token is bucketed by the customer it names, so each shopper has their own budget.
        /// Everything else shares one administrator bucket, whether it presented the shared API key or an
        /// administrator token, because they grant the same access and an administrator token is unique
        /// per issuance.
        /// </remarks>
        protected static string BuildClientId(AuthenticateResult result)
        {
            var customerId = result.Principal?.FindFirst(ApiRestDefaults.CustomerIdClaim)?.Value;
            if (!string.IsNullOrEmpty(customerId))
                return $"customer:{customerId}";

            return "admin";
        }

        /// <summary>
        /// Consume one unit of the client's rate limit
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="clientId">Client identifier</param>
        /// <returns>True when the request is within the limit, false when the response was already written</returns>
        protected virtual async Task<bool> TryConsumeAsync(HttpContext context, string clientId)
        {
            var settings = context.RequestServices.GetService(typeof(ApiRestSettings)) as ApiRestSettings;
            var limit = settings?.RateLimitPerMinute ?? 0;
            if (limit <= 0)
                return true;

            var now = DateTime.UtcNow;
            var entry = _cache.GetOrCreate(clientId, e =>
            {
                var expiresAt = now.AddMinutes(1);
                e.AbsoluteExpiration = expiresAt;
                return new RateLimitEntry { Count = 0, ExpiresAtUtc = expiresAt };
            });

            if (entry.Count >= limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                var retryAfter = (int)Math.Ceiling((entry.ExpiresAtUtc - now).TotalSeconds);
                context.Response.Headers["Retry-After"] = retryAfter.ToString();
                await context.Response.WriteAsJsonAsync(new { error = "Too many requests", retryAfterSeconds = retryAfter });
                return false;
            }

            entry.Count++;
            // renew the count, keeping the expiration established when the entry was created
            _cache.Set(clientId, entry, entry.ExpiresAtUtc);

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the read operations are protected by the API key
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>True when the API key is configured to guard the read operations too</returns>
        protected static bool RequiresApiKeyForReads(HttpContext context)
            => (context.RequestServices.GetService(typeof(ApiRestSettings)) as ApiRestSettings)
                ?.RequireApiKeyForReads == true;
    }
}
