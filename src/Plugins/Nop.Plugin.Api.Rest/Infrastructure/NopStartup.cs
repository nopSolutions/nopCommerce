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
        /// <summary>
        /// Gets the Swagger document group name for the back office API
        /// </summary>
        public static string BackendSwaggerGroup => "api-backend";

        /// <summary>
        /// Gets the Swagger document group name for the public store API
        /// </summary>
        public static string FrontendSwaggerGroup => "api-frontend";

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
                // Two documents, split the same way the official nopCommerce Web API splits itself, so a
                // client written against either one finds the paths it already knows under the same name.
                // Which operations land in which document comes from ApiRestDefaults.IsFrontendPath, the
                // same predicate the middleware enforces the scope with, so the published contract cannot
                // promise a credential the runtime then refuses.
                options.SwaggerDoc(BackendSwaggerGroup, new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "nopCommerce Web API for backend",
                    Version = "v1",
                    Description = BackendDescription
                });

                options.SwaggerDoc(FrontendSwaggerGroup, new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "nopCommerce Web API for public store",
                    Version = "v1",
                    Description = FrontendDescription
                });

                // (Optional) Include XML comments if you generate them for the plugin assembly
                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);

                //Each document describes this plugin's own half of the API. SwaggerGen would otherwise pick
                //up the attribute routed controllers of every other loaded plugin and publish them too.
                options.DocInclusionPredicate((documentName, apiDescription) =>
                {
                    if (apiDescription.ActionDescriptor is not ControllerActionDescriptor actionDescriptor
                        || actionDescriptor.ControllerTypeInfo.Assembly != typeof(ApiRestDefaults).Assembly)
                        return false;

                    var isFrontend = ApiRestDefaults.IsFrontendPath(apiDescription.RelativePath);

                    return documentName == FrontendSwaggerGroup ? isFrontend : !isFrontend;
                });

                //two schemes, matching the two slots the handler reads. Each header carries one format and is never
                //read as the other, so the document advertises exactly that rather than three
                //overlapping ways to authenticate.
                options.AddSecurityDefinition(ApiRestDefaults.ApiKeySchemeId, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "A long random string configured on this store. Grants admin level access to "
                        + "every record and never expires. Generate one on the plugin configuration page, and "
                        + "use it for server-to-server calls that would rather not hold a token. Send it in the "
                        + $"{ApiRestDefaults.ApiKeyHeaderName} header only; it is not accepted as a bearer token. "
                        + "Anyone holding it can modify the catalog, orders and customers.",
                    Name = ApiRestDefaults.ApiKeyHeaderName,
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                });
                //published as an http/bearer scheme rather than a second apiKey scheme. Declaring it as
                //apiKey advertises a raw header value, which forces the caller to type the whole
                //"Bearer <token>" string by hand and renders in the UI as a second, identical apiKey entry.
                options.AddSecurityDefinition(ApiRestDefaults.BearerSchemeId, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "A signed JWT with an expiry. In this document a token from "
                        + "POST /api/rest/token grants admin level access to every record; the API key above "
                        + "grants the same and never expires. Tokens expire, the response says when, and there "
                        + "is no refresh: post the credentials again for a new one.",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                //apply the requirement per operation instead of on the document. A document level
                //requirement is emitted as root level "security", which locks every endpoint, including
                //the public read operations.
                options.OperationFilter<ApiKeySecurityOperationFilter>();

                //the two documents accept different credentials, but AddSecurityDefinition cannot tell
                //them apart, so the public store document is corrected here: it drops the API key, which
                //its operations refuse with 403, and gets a Bearer description written for it.
                options.DocumentFilter<FrontendSecuritySchemeFilter>();
            });
        }

        public void Configure(IApplicationBuilder application)
        {
            var logger = application.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var log = logger?.CreateLogger("NopStartup.ApiRest");
            log?.LogInformation("=== Api.Rest NopStartup.Configure called ===");

            // Apply credential and rate-limit middleware for plugin API routes only. Both token endpoints
            // live under /api/rest like every other operation, so the segment check alone covers them; the
            // middleware still treats them specially by skipping the credential check, which is what makes
            // them reachable without holding a credential in the first place.
            application.UseWhen(context =>
                context.Request.Path.StartsWithSegments("/api/rest", StringComparison.OrdinalIgnoreCase),
                appBranch =>
            {
                appBranch.UseMiddleware<ApiKeyRateLimitMiddleware>();
            });

            // Serve Swagger JSON and UI at standard routes (no custom path prefix)
            log?.LogInformation("Registering Swagger middleware");
            application.UseSwagger();

            application.UseSwaggerUI(c =>
            {
                //registered twice on purpose: Swagger UI turns a second endpoint into the "Select a
                //definition" dropdown at the top of the page, which is how the official demo offers the
                //backend and public store documents side by side without a separate page for each
                c.SwaggerEndpoint($"/{ApiRestDefaults.SwaggerBackendJsonPath}", "nopCommerce Web API for backend v1");
                c.SwaggerEndpoint($"/{ApiRestDefaults.SwaggerFrontendJsonPath}", "nopCommerce Web API for public store v1");
                c.RoutePrefix = "swagger/api-rest"; // URL: /swagger/api-rest
                log?.LogInformation("SwaggerUI configured with RoutePrefix: {0}", c.RoutePrefix);
            });

            log?.LogInformation("=== Api.Rest NopStartup.Configure completed ===");
        }

        /// <summary>
        /// Gets the description published at the top of the back office document
        /// </summary>
        /// <remarks>
        /// The first thing a back office integrator reads, so it explains both of the credentials this
        /// document accepts and states plainly that a customer token is refused here.
        /// </remarks>
        private const string BackendDescription = """
            The back office half of the REST API: the catalog, orders, shipments, customers, categories,
            manufacturers, logs and metafields, plus the token endpoint that issues an admin token.

            ## Getting a credential

            Everything here is a back office operation, so it needs an admin level credential. Two are
            accepted and neither is valid in the other's header.

            - **A bearer token**, as `Authorization: Bearer <token>`. Post administrator credentials to
              `POST /api/rest/token` and read the `token` field of the response. It expires; the response
              says when, and there is no refresh, so post the credentials again for a new one.
            - **The shared API key**, as `X-Api-Key: <key>`. A long random string you generate on the
              plugin configuration page (Administration > Plugins > REST API > Configure). It grants the
              same access as an admin token and never expires, which suits background jobs and scheduled
              integrations. Treat it as a password: anyone holding it can modify the catalog, orders and
              customers.

            **A customer token is refused here with 403.** It is signed with the same key and is only
            meant for the public store document.

            ## Rate limiting

            Requests are limited per minute, configurable on the settings page. Admin level callers share
            one bucket, and a request with no valid credential is counted per IP.
            """;

        /// <summary>
        /// Gets the description published at the top of the public store document
        /// </summary>
        /// <remarks>
        /// Written per document rather than shared, because each one is read by a different audience: an
        /// integrator building a storefront needs the customer token and never needs the API key.
        /// </remarks>
        private const string FrontendDescription = """
            The public store half of the REST API: the token endpoint that issues a customer token, that
            customer's own profile, addresses, orders and wishlist, and a storefront view of the catalog.

            ## Getting a credential

            - **A customer token**, as `Authorization: Bearer <token>`. Post the customer's credentials to
              `POST /api/rest/customer/token` and read the `token` field of the response. It expires; the
              response says when, and there is no refresh, so post the credentials again for a new one.

            The token is scoped to the customer it was issued to. There is no customer identifier anywhere
            in these paths, query strings or bodies, so a caller cannot reach another shopper's data by
            changing a value.

            **The shared API key is refused here with 403**, as is an admin token.

            ## Which requests need one

            The `/api/rest/customer/me/*` routes always need a customer token, whatever the read setting says.
            The storefront catalog projection follows it: anonymous while "Require a credential for reads"
            is off, and behind a customer token once it is on.

            ## Choosing a store on a multi store install

            The storefront catalog projection accepts an `X-Store-Id` header naming the store being
            browsed, which is how the official public store API has the client identify itself. Send it
            on every storefront request. Omitting it leaves the store unfiltered, and a product that
            belongs to another store is reported as missing rather than returned.
            """;

        /// <summary>
        /// Gets order of this startup configuration implementation. Must be less than 900 (NopEndpoints),
        /// otherwise the middleware is registered after the terminal UseEndpoints and never runs.
        /// </summary>
        public int Order => 700;
    }
}