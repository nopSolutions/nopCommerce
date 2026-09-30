using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
    /// Applies the per client rate limit and requires the API key on the operations that are protected by it
    /// </summary>
    /// <remarks>
    /// The key itself is validated by <see cref="Security.ApiRestApiKeyAuthenticationHandler"/>, so that the
    /// same credential is accepted from either supported header and validated in constant time.
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
            // Only protect plugin API routes. The swagger UI and its JSON document are served from
            // root level paths, so they never reach this middleware and need no exemption here.
            var path = context.Request.Path.Value ?? string.Empty;
            if (!ApiRestDefaults.IsApiPath(path))
            {
                await _next(context);
                return;
            }

            var isAuthenticated = await context.AuthenticateAsync(ApiRestDefaults.AuthenticationSchemeName);
            var requiresApiKey = !ApiRestDefaults.IsTokenPath(path)
                && ApiRestDefaults.RequiresApiKey(context.Request.Method, RequiresApiKeyForReads(context));

            // Rate limiting runs before the API key is enforced, so that a rejected caller cannot spend
            // attempts. Clients are identified by key only once the key has actually validated,
            // otherwise varying the header would hand out a fresh bucket per attempt. The key is hashed,
            // because a bearer client leaves the X-Api-Key header empty and would otherwise be bucketed
            // together with every other bearer client, and so that no credential is held as a cache key.
            var clientId = isAuthenticated.Succeeded
                ? $"apiKey:{HashClientKey(ApiRestDefaults.GetTokenFromRequest(context.Request.Headers))}"
                : $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

            if (!await TryConsumeAsync(context, clientId))
                return;

            if (requiresApiKey && !isAuthenticated.Succeeded)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "API key missing or invalid. Send it in the 'X-Api-Key' header or as 'Authorization: Bearer <key>'."
                });
                return;
            }

            await _next(context);
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

        /// <summary>
        /// Reduce a client credential to a value that is safe to use as a rate limit bucket key
        /// </summary>
        /// <param name="key">Presented API key</param>
        /// <returns>The hex encoded hash of the key</returns>
        protected static string HashClientKey(string key)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(key ?? string.Empty));

            return Convert.ToHexString(bytes);
        }
    }
}
