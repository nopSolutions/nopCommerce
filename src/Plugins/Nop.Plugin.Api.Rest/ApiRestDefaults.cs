using System;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Api.Rest;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class ApiRestDefaults
{
    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.Api.Rest.Configure";

    /// <summary>
    /// Gets the name of the folder the plugin is deployed to
    /// </summary>
    /// <remarks>
    /// Must match the OutputPath of the plugin project ("$(SolutionDir)\Presentation\Nop.Web\Plugins\{this value}")
    /// </remarks>
    public static string OutputFolderName => "Api.Rest";

    /// <summary>
    /// Gets the relative path of the Swagger UI page
    /// </summary>
    public static string SwaggerUiPath => "swagger/api-rest/index.html";

    /// <summary>
    /// Gets the relative path of the Swagger JSON document describing the back office API
    /// </summary>
    public static string SwaggerBackendJsonPath => "swagger/api-backend/swagger.json";

    /// <summary>
    /// Gets the relative path of the Swagger JSON document describing the public store API
    /// </summary>
    /// <remarks>
    /// The Swagger UI page lists both documents in a "Select a definition" dropdown, the same as the
    /// official nopCommerce Web API demo. Neither document is served on its own page, because the choice
    /// between them belongs to whoever is integrating rather than to the URL.
    /// </remarks>
    public static string SwaggerFrontendJsonPath => "swagger/api-frontend/swagger.json";

    /// <summary>
    /// Gets the identifier of the OpenAPI security scheme for the shared API key header
    /// </summary>
    public static string ApiKeySchemeId => "ApiKey";

    /// <summary>
    /// Gets the identifier of the OpenAPI security scheme for the bearer token
    /// </summary>
    public static string BearerSchemeId => "Bearer";

    /// <summary>
    /// The slot a credential arrived in
    /// </summary>
    /// <remarks>
    /// The two slots accept different formats and are never read as each other, so the slot has to
    /// travel with the value rather than being inferred from it.
    /// </remarks>
    public enum CredentialSlot
    {
        /// <summary>
        /// No credential was presented
        /// </summary>
        None = 0,

        /// <summary>
        /// The shared API key, sent in the <see cref="ApiKeyHeaderName"/> header
        /// </summary>
        ApiKeyHeader,

        /// <summary>
        /// A signed token, sent as <c>Authorization: Bearer &lt;token&gt;</c>
        /// </summary>
        BearerToken
    }

    /// <summary>
    /// A credential as it arrived, together with the slot it arrived in
    /// </summary>
    /// <param name="Slot">Where the credential was found</param>
    /// <param name="Value">The raw value, null when no credential was presented</param>
    public readonly record struct PresentedCredential(CredentialSlot Slot, string Value);

    /// <summary>
    /// Gets the name of the authentication scheme that validates the presented credential
    /// </summary>
    public static string AuthenticationSchemeName => "ApiRestApiKey";

    /// <summary>
    /// Gets the claim that records which kind of credential authenticated the request
    /// </summary>
    public static string CredentialTypeClaim => "nop:credential";

    /// <summary>
    /// Gets the claim that carries the identifier of the customer a customer token was issued to
    /// </summary>
    /// <remarks>
    /// Named to match the claim the official nopCommerce Web API puts in its own token, so a token
    /// issued here reads the same way
    /// </remarks>
    public static string CustomerIdClaim => "CustomerId";

    /// <summary>
    /// The credential was the shared API key, which grants the admin level of access
    /// </summary>
    public static string CredentialTypeApiKey => "ApiKey";

    /// <summary>
    /// The credential was a customer token, which only grants access to the issuing customer's own data
    /// </summary>
    public static string CredentialTypeCustomerToken => "CustomerToken";

    /// <summary>
    /// Gets the name of the header that carries the shared API key
    /// </summary>
    /// <remarks>
    /// Must match the header read by the API key authentication handler
    /// </remarks>
    public static string ApiKeyHeaderName => "X-Api-Key";

    /// <summary>
    /// Gets the name of the header that carries a bearer token
    /// </summary>
    /// <remarks>
    /// Carries a signed token only. The shared API key is not accepted here, so a caller that has both
    /// credentials cannot present the wrong one by mistake.
    /// </remarks>
    public static string AuthorizationHeaderName => "Authorization";

    /// <summary>
    /// Gets the prefix of an Authorization header value that carries a bearer token
    /// </summary>
    public static string BearerPrefix => "Bearer ";

    /// <summary>
    /// Gets the route prefix of the plugin API, relative and without a leading slash so that it can be
    /// matched against <c>ApiDescription.RelativePath</c> as well as against <c>HttpRequest.Path</c>
    /// </summary>
    public static string ApiRoutePrefix => "api/rest";

    /// <summary>
    /// Gets the route of the endpoint that hands out an administrator token, in exchange for admin credentials
    /// </summary>
    public static string TokenRoute => "api/rest/token";

    /// <summary>
    /// Gets the route of the endpoint that hands out a customer token, in exchange for store credentials
    /// </summary>
    public static string CustomerTokenRoute => "api/rest/customer/token";

    /// <summary>
    /// Gets the route prefix of the endpoints that act on the authenticated customer's own data
    /// </summary>
    public static string CustomerScopeRoutePrefix => "api/rest/customer/me";

    /// <summary>
    /// Gets the route prefix of the endpoints that project the catalog as a browsing client sees it
    /// </summary>
    /// <remarks>
    /// Part of the public store side of the API, so it is served alongside <see cref="CustomerScopeRoutePrefix"/>
    /// rather than with the back office routes.
    /// </remarks>
    public static string StorefrontScopeRoutePrefix => "api/rest/store";

    /// <summary>
    /// Gets the name of the header a public store client sends to name the store it is browsing
    /// </summary>
    /// <remarks>
    /// The public store API of the official nopCommerce Web API has the client name its store on every
    /// request rather than in each call, and this plugin follows that. The host does the real work: the
    /// header only narrows what the caller may see, so a missing or wrong value cannot widen access.
    /// </remarks>
    public static string StoreIdHeaderName => "X-Store-Id";

    /// <summary>
    /// Checks whether a path targets the plugin API
    /// </summary>
    /// <param name="path">
    /// Request path or relative API path. A leading slash is tolerated so both forms can be passed.
    /// </param>
    /// <returns>True when the path belongs to the plugin API</returns>
    public static bool IsApiPath(string path)
        => Normalize(path).StartsWith(ApiRoutePrefix, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a path targets the endpoint that hands out an administrator token
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path targets an administrator token endpoint</returns>
    public static bool IsAdminTokenPath(string path)
        => string.Equals(Normalize(path), TokenRoute, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a path targets the endpoint that hands out a customer token
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path targets a customer token endpoint</returns>
    public static bool IsCustomerTokenPath(string path)
        => string.Equals(Normalize(path), CustomerTokenRoute, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a path targets one of the credential endpoints, which are exempt from the
    /// credential checks because they are how a caller obtains a credential in the first place
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path targets a token endpoint</returns>
    /// <remarks>
    /// Both spellings of both endpoints are checked here, so every caller that consults this predicate
    /// stays in step when a credential endpoint is added. Missing one would make the endpoint that hands
    /// out a credential itself require that credential, and so unreachable.
    /// </remarks>
    public static bool IsTokenPath(string path)
        => IsAdminTokenPath(path) || IsCustomerTokenPath(path);

    /// <summary>
    /// Checks whether a path targets the endpoints that act on the authenticated customer's own data
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path is scoped to the calling customer</returns>
    public static bool IsCustomerScopePath(string path)
        => Normalize(path).StartsWith(CustomerScopeRoutePrefix, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a path targets the endpoints that project the catalog for a browsing client
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path is a public store projection</returns>
    public static bool IsStorefrontScopePath(string path)
        => Normalize(path).StartsWith(StorefrontScopeRoutePrefix, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a path belongs to the public store side of the API
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path is served to a public store client</returns>
    /// <remarks>
    /// The split mirrors the official nopCommerce Web API, which serves the back office from
    /// <c>api-backend</c> and the public store from <c>api-frontend</c>. This predicate is the single
    /// source of that decision: the middleware enforces it, the two Swagger documents are built from it,
    /// and the published security requirement is derived from it, so the documented contract cannot
    /// disagree with what is actually enforced.
    /// </remarks>
    public static bool IsFrontendPath(string path)
        => IsCustomerTokenPath(path) || IsCustomerScopePath(path) || IsStorefrontScopePath(path);

    /// <summary>
    /// Checks whether a path belongs to the back office side of the API
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path is a back office operation</returns>
    /// <remarks>
    /// The complement of <see cref="IsFrontendPath"/> within the plugin API. Token endpoints are on
    /// neither side, because both documents publish their own and neither credential admits the other.
    /// </remarks>
    public static bool IsAdminApiPath(string path)
        => IsApiPath(path) && !IsFrontendPath(path) && !IsTokenPath(path);

    /// <summary>
    /// Reads the credential presented by a request, and the slot it arrived in
    /// </summary>
    /// <param name="headers">Request headers</param>
    /// <returns>
    /// The presented credential, or a slot of <see cref="CredentialSlot.None"/> when the request carries none
    /// </returns>
    /// <remarks>
    /// One format per slot: the Authorization header carries a signed token and the dedicated header
    /// carries the shared API key. Neither is ever read as the other. Accepting the raw key as a bearer
    /// token as well would mean one header held two unrelated token formats, which is exactly the
    /// ambiguity this split removes. Shared by the authentication handler and the rate limiter, which
    /// must agree on both the value and the slot.
    /// </remarks>
    public static PresentedCredential GetCredential(IHeaderDictionary headers)
    {
        if (headers == null)
            return default;

        if (headers.TryGetValue(AuthorizationHeaderName, out var authorization))
        {
            var value = authorization.ToString();
            if (value.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var token = value[BearerPrefix.Length..].Trim();
                if (token.Length > 0)
                    return new PresentedCredential(CredentialSlot.BearerToken, token);
            }
        }

        if (headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
        {
            var key = apiKey.ToString().Trim();
            if (key.Length > 0)
                return new PresentedCredential(CredentialSlot.ApiKeyHeader, key);
        }

        return default;
    }

    /// <summary>
    /// Checks whether an HTTP method changes data
    /// </summary>
    /// <param name="method">HTTP request method</param>
    /// <returns>True for the methods that create, update or delete a resource</returns>
    public static bool IsMutation(string method)
        => HttpMethods.IsPost(method) || HttpMethods.IsPut(method)
            || HttpMethods.IsPatch(method) || HttpMethods.IsDelete(method);

    /// <summary>
    /// Checks whether an operation is guarded by the API key
    /// </summary>
    /// <param name="method">HTTP request method</param>
    /// <param name="requireApiKeyForReads">Whether the read operations are guarded too</param>
    /// <returns>True when the operation needs the API key</returns>
    /// <remarks>
    /// The single source of truth for the rule, so the runtime check, the published Swagger document and
    /// the methods reported by the token endpoint can never disagree about what is protected.
    /// </remarks>
    public static bool RequiresApiKey(string method, bool requireApiKeyForReads)
        => IsMutation(method) || requireApiKeyForReads;

    /// <summary>
    /// Gets the HTTP methods that are guarded by the API key under the passed configuration
    /// </summary>
    /// <param name="requireApiKeyForReads">Whether the read operations are guarded too</param>
    /// <returns>The guarded HTTP methods</returns>
    public static string[] GetSecuredMethods(bool requireApiKeyForReads)
        => requireApiKeyForReads
            ? ["GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS", "TRACE"]
            : ["POST", "PUT", "PATCH", "DELETE"];

    /// <summary>
    /// Trims the leading and trailing slashes so that request paths and relative API paths compare equally
    /// </summary>
    /// <param name="path">Path to normalize</param>
    /// <returns>Normalized path</returns>
    private static string Normalize(string path)
        => (path ?? string.Empty).Trim('/');
}
