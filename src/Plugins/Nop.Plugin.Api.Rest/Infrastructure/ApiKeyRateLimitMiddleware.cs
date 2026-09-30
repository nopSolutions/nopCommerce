using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Nop.Plugin.Api.Rest.Infrastructure
{
    internal class RateLimitEntry
    {
        public int Count { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }

    public class ApiKeyRateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly string? _apiKeyConfigured;
        private readonly int _limitPerMinute;

        public ApiKeyRateLimitMiddleware(RequestDelegate next, IMemoryCache cache, IConfiguration configuration)
        {
            _next = next;
            _cache = cache;
            _apiKeyConfigured = configuration["Plugins:ApiRest:ApiKey"]; // optional
            // allow override per config, default 60
            if (!int.TryParse(configuration["Plugins:ApiRest:RateLimitPerMinute"], out _limitPerMinute))
                _limitPerMinute = 60;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only protect plugin API routes. The swagger UI and its JSON document are served from
            // root level paths, so they never reach this middleware and need no exemption here.
            var path = context.Request.Path.Value ?? string.Empty;
            if (!path.StartsWith("/api/rest", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var providedKey = context.Request.Headers["X-Api-Key"].ToString();
            var keyIsValid = !string.IsNullOrEmpty(_apiKeyConfigured)
                && !string.IsNullOrEmpty(providedKey)
                && string.Equals(providedKey, _apiKeyConfigured, StringComparison.Ordinal);

            // API key enforcement (optional): if configured, require matching header
            if (!string.IsNullOrEmpty(_apiKeyConfigured) && !keyIsValid)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "API key missing or invalid" });
                return;
            }

            // Writing data is only allowed with a valid API key, and fails closed when none is configured.
            // Reads stay open so the catalog can be browsed and the swagger UI explored without credentials.
            if (IsMutation(context.Request.Method))
            {
                if (string.IsNullOrEmpty(_apiKeyConfigured))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Write operations are disabled because no API key is configured" });
                    return;
                }

                if (!keyIsValid)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "API key missing or invalid" });
                    return;
                }
            }

            // Identify client by API key (if provided) or remote IP
            var clientId = !string.IsNullOrEmpty(providedKey) ? $"apiKey:{providedKey}" : $"ip:{context.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

            var now = DateTime.UtcNow;
            var entry = _cache.GetOrCreate(clientId, e =>
            {
                var expiresAt = now.AddMinutes(1);
                e.AbsoluteExpiration = expiresAt;
                return new RateLimitEntry { Count = 0, ExpiresAtUtc = expiresAt };
            });

            // increment
            if (entry.Count >= _limitPerMinute)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                var retryAfter = (int)Math.Ceiling((entry.ExpiresAtUtc - now).TotalSeconds);
                context.Response.Headers["Retry-After"] = retryAfter.ToString();
                await context.Response.WriteAsJsonAsync(new { error = "Too many requests", retryAfterSeconds = retryAfter });
                return;
            }

            entry.Count++;
            // update cache with renewed count (expiration preserved by memory cache entry options)
            _cache.Set(clientId, entry, entry.ExpiresAtUtc);

            await _next(context);
        }

        /// <summary>
        /// Checks whether the request method changes data
        /// </summary>
        /// <param name="method">HTTP request method</param>
        /// <returns>True for the methods that create, update or delete a resource</returns>
        protected static bool IsMutation(string method)
            => HttpMethods.IsPost(method) || HttpMethods.IsPut(method)
                || HttpMethods.IsPatch(method) || HttpMethods.IsDelete(method);
    }
}
