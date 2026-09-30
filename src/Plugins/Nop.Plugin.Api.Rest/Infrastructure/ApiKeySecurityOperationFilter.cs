using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nop.Plugin.Api.Rest.Infrastructure
{
    /// <summary>
    /// Marks only the operations that are actually protected by the API key as secured.
    /// </summary>
    /// <remarks>
    /// A security requirement registered on the Swagger document (or inherited from one) is emitted as
    /// OpenAPI root level "security", which applies to every operation and makes the UI show a lock icon on
    /// all endpoints. The requirement is therefore applied per operation instead, and it mirrors the decision
    /// made by <c>ApiKeyRateLimitMiddleware</c> so the published contract never understates the protection.
    /// </remarks>
    public class ApiKeySecurityOperationFilter : IOperationFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ApiKeySecurityOperationFilter(IServiceProvider serviceProvider)
        {
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
            if (ApiRestDefaults.IsCustomerScopePath(relativePath))
            {
                operation.Security.Add(CreateRequirement(ApiRestDefaults.CustomerTokenSchemeId));
                return;
            }

            if (!RequiresApiKey(context))
                return;

            //two entries, because a list of requirements means "any of". One entry holding both schemes
            //would mean "both of them", which is stricter than what the handler accepts.
            operation.Security.Add(CreateRequirement(ApiRestDefaults.SecuritySchemeId));
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
        /// Checks whether the operation is guarded by the API key middleware
        /// </summary>
        /// <param name="context">Operation filter context</param>
        /// <returns>True for the operations that require the API key</returns>
        protected virtual bool RequiresApiKey(OperationFilterContext context)
        {
            var settings = _serviceProvider.GetService(typeof(ApiRestSettings)) as ApiRestSettings;

            return ApiRestDefaults.RequiresApiKey(context.ApiDescription.HttpMethod,
                settings?.RequireApiKeyForReads == true);
        }
    }
}
