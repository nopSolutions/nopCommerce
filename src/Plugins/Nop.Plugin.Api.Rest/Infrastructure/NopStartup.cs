using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Rest.Security;

namespace Nop.Plugin.Api.Rest.Infrastructure
{
    /// <summary>
    /// Plugin startup for registering services and middleware. All registrations remain inside the plugin assembly.
    /// </summary>
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // register memory cache used for rate limiting
            services.AddMemoryCache();

            // register the API key authentication scheme. The host already calls UseAuthentication,
            // and registering an extra scheme leaves the cookie based default scheme untouched, so the
            // admin area keeps working exactly as before.
            services.AddAuthentication()
                .AddScheme<ApiRestApiKeyAuthenticationOptions, ApiRestApiKeyAuthenticationHandler>(
                    ApiRestDefaults.AuthenticationSchemeName, options => { });

            // Register Swagger for this plugin only. Keep route template under a plugin-specific prefix to avoid clashes.
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "nopCommerce REST API (Plugin)", Version = "v1" });
                // (Optional) Include XML comments if you generate them for the plugin assembly
                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);

                //the document describes this plugin only. SwaggerGen otherwise picks up the attribute
                //routed controllers of every other loaded plugin and publishes them under this route.
                options.DocInclusionPredicate((_, apiDescription) =>
                    apiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor
                    && actionDescriptor.ControllerTypeInfo.Assembly == typeof(ApiRestDefaults).Assembly);

                //describe the credentials the plugin actually validates. The handler accepts either header,
                //so both are published and the operation filter lists them as alternatives.
                options.AddSecurityDefinition(ApiRestDefaults.SecuritySchemeId, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = $"API key sent in the {ApiRestDefaults.ApiKeyHeaderName} header. Manage it on the plugin configuration page.",
                    Name = ApiRestDefaults.ApiKeyHeaderName,
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                });
                //must be declared as an http/bearer scheme rather than a second apiKey scheme. Declaring it as
                //apiKey advertises a raw header value, which forces the caller to type the whole
                //"Bearer <key>" string by hand and renders in the UI as a second, identical apiKey entry.
                options.AddSecurityDefinition(ApiRestDefaults.BearerSchemeId, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "API key sent as a bearer token. Accepted as an alternative to the X-Api-Key header.",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                //apply the requirement per operation instead of on the document. A document level
                //requirement is emitted as root level "security", which locks every endpoint, including
                //the public read operations.
                options.OperationFilter<ApiKeySecurityOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder application)
        {
            var logger = application.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var log = logger?.CreateLogger("NopStartup.ApiRest");
            log?.LogInformation("=== Api.Rest NopStartup.Configure called ===");

            // Apply API-key and rate-limit middleware for plugin API routes only
            application.UseWhen(context => context.Request.Path.StartsWithSegments("/api/rest", StringComparison.OrdinalIgnoreCase), appBranch =>
            {
                appBranch.UseMiddleware<ApiKeyRateLimitMiddleware>();
            });

            // Serve Swagger JSON and UI at standard routes (no custom path prefix)
            log?.LogInformation("Registering Swagger middleware");
            application.UseSwagger();

            application.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "nopCommerce REST API (Plugin) v1");
                c.RoutePrefix = "swagger/api-rest"; // URL: /swagger/api-rest
                log?.LogInformation("SwaggerUI configured with RoutePrefix: {0}", c.RoutePrefix);
            });

            log?.LogInformation("=== Api.Rest NopStartup.Configure completed ===");
        }

        /// <summary>
        /// Gets order of this startup configuration implementation. Must be less than 900 (NopEndpoints),
        /// otherwise the middleware is registered after the terminal UseEndpoints and never runs.
        /// </summary>
        public int Order => 700;
    }
}
