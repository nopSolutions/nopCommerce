using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nop.Plugin.Api.Rest.Infrastructure
{
    /// <summary>
    /// Marks only the operations that are actually protected as secured, with the credential they accept
    /// </summary>
    /// <remarks>
    /// A security requirement registered on the Swagger document (or inherited from one) is emitted as
    /// OpenAPI root level "security", which applies to every operation and makes the UI show a lock icon on
    /// all endpoints. The requirement is therefore applied per operation instead. Which credential is
    /// advertised comes from <c>ApiRestDefaults.IsFrontendPath</c>, the same predicate
    /// <c>ApiKeyRateLimitMiddleware</c> enforces the scope with, so an operation can never be published
    /// with a credential the runtime then refuses.
    /// </remarks>
    public class ApiKeySecurityOperationFilter : IOperationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public ApiKeySecurityOperationFilter(IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //clear any inherited requirement first, so unprotected operations are never locked
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Clear();

            var relativePath = context.ApiDescription.RelativePath ?? string.Empty;

            //the token endpoints are how a caller obtains a credential, so requiring one there would
            //make them unreachable
            if (!ApiRestDefaults.IsApiPath(relativePath) || ApiRestDefaults.IsTokenPath(relativePath))
                return;

            //the customer scoped operations refuse the shared API key, so they are published with the
            //credential they actually accept
            if (ApiRestDefaults.IsFrontendPath(relativePath))
            {
                // The customer scoped routes expose one customer's own data, so they need a customer token
                // whatever the read setting says. The storefront projection follows the setting instead,
                // which is what ApiKeyRateLimitMiddleware enforces: publishing a requirement the runtime
                // does not apply would put a lock icon on an operation anyone may call anonymously, and
                // asking a storefront client for a token it does not need is the same mistake in reverse.
                if (ApiRestDefaults.IsCustomerScopePath(relativePath) || RequiresApiKey(context))
                    operation.Security.Add(CreateRequirement(ApiRestDefaults.BearerSchemeId));

                return;
            }

            if (!RequiresApiKey(context))
                return;

            //two entries, because a list of requirements means "any of". One entry holding both schemes
            //would mean "both of them", which is stricter than what the handler accepts.
            operation.Security.Add(CreateRequirement(ApiRestDefaults.ApiKeySchemeId));
            operation.Security.Add(CreateRequirement(ApiRestDefaults.BearerSchemeId));
        }

        /// <summary>
        /// Creates a security requirement referencing the passed scheme
        /// </summary>
        /// <param name="schemeId">Identifier of the security scheme</param>
        /// <returns>The security requirement</returns>
        protected static OpenApiSecurityRequirement CreateRequirement(string schemeId)
        {
            return new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = schemeId
                        }
                    },
                    Array.Empty<string>()
                }
            };
        }

        /// <summary>
        /// Checks whether the operation is guarded by the credential middleware
        /// </summary>
        /// <param name="context">Operation filter context</param>
        /// <returns>True for the operations that require a credential</returns>
        protected virtual bool RequiresApiKey(OperationFilterContext context)
        {
            var settings = GetSettings();

            return ApiRestDefaults.RequiresApiKey(context.ApiDescription.HttpMethod,
                settings?.RequireApiKeyForReads == true);
        }

        /// <summary>
        /// Reads the settings for the request being documented
        /// </summary>
        /// <returns>The settings, or null when they cannot be resolved</returns>
        /// <remarks>
        /// The settings are registered as scoped, so they have to be taken from the request scope. Resolving
        /// them from the injected root provider, which is what this filter used to do, yields one instance
        /// for the lifetime of the application instead of one per request. The filter then published a
        /// document built from whatever the setting was when the document was first generated, while
        /// <c>ApiKeyRateLimitMiddleware</c> resolved the same settings per request and enforced the current
        /// value. The visible symptom was a read operation answering 401 at runtime while the Swagger
        /// document showed it as unsecured, so the UI offered no Authorize prompt and the generated curl
        /// carried no credential at all.
        /// </remarks>
        protected virtual ApiRestSettings GetSettings()
        {
            return _httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(ApiRestSettings))
                as ApiRestSettings
                ?? _serviceProvider.GetService(typeof(ApiRestSettings)) as ApiRestSettings;
        }
    }
}
