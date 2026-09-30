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
            // Allow the plugin swagger UI and json to be served without API key / rate limiting
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/plugins/nop-plugin-api-rest/swagger", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("/plugins/nop-plugin-api-rest/swagger/", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Only protect plugin API routes
            if (!path.StartsWith("/api/rest", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // API key enforcement (optional): if configured, require matching header
            var providedKey = context.Request.Headers["X-Api-Key"].ToString();
            if (!string.IsNullOrEmpty(_apiKeyConfigured))
            {
                if (string.IsNullOrEmpty(providedKey) || !string.Equals(providedKey, _apiKeyConfigured, StringComparison.Ordinal))
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
    }
}
