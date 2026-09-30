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
    /// Gets the relative path of the Swagger JSON document
    /// </summary>
    public static string SwaggerJsonPath => "swagger/v1/swagger.json";

    /// <summary>
    /// Gets the identifier of the OpenAPI security scheme for the API key header
    /// </summary>
    public static string SecuritySchemeId => "ApiKey";

    /// <summary>
    /// Gets the identifier of the OpenAPI security scheme for the Authorization header
    /// </summary>
    public static string BearerSchemeId => "Bearer";

    /// <summary>
    /// Gets the identifier of the OpenAPI security scheme for a customer token
    /// </summary>
    public static string CustomerTokenSchemeId => "CustomerToken";

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
    /// Gets how long an issued customer token stays valid
    /// </summary>
    /// <remarks>
    /// Seven days, matching the lifetime the official nopCommerce Web API issues its own tokens for
    /// </remarks>
    public static TimeSpan CustomerTokenLifetime => TimeSpan.FromDays(7);

    /// <summary>
    /// Gets the name of the header that carries the API key
    /// </summary>
    /// <remarks>
    /// Must match the header read by the API key authentication handler
    /// </remarks>
    public static string ApiKeyHeaderName => "X-Api-Key";

    /// <summary>
    /// Gets the name of the header that carries the API key as a bearer token
    /// </summary>
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
    /// Gets the route of the endpoint that hands out the API key in exchange for admin credentials
    /// </summary>
    public static string TokenRoute => "api/rest/token";

    /// <summary>
    /// Gets the route of the endpoint that hands out a customer token in exchange for store credentials
    /// </summary>
    public static string CustomerTokenRoute => "api/rest/customer/token";

    /// <summary>
    /// Gets the route prefix of the endpoints that act on the authenticated customer's own data
    /// </summary>
    public static string CustomerScopeRoutePrefix => "api/rest/customer/me";

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
    /// Checks whether a path targets one of the credential endpoints, which are exempt from the
    /// credential checks because they are how a caller obtains a credential in the first place
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path targets a token endpoint</returns>
    /// <remarks>
    /// Both token routes are checked here, so every caller that consults this predicate stays in step
    /// when a credential endpoint is added. Missing one would make the endpoint that hands out a
    /// credential itself require that credential, and so unreachable.
    /// </remarks>
    public static bool IsTokenPath(string path)
    {
        var normalized = Normalize(path);

        return string.Equals(normalized, TokenRoute, StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, CustomerTokenRoute, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks whether a path targets the endpoints that act on the authenticated customer's own data
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path is scoped to the calling customer</returns>
    public static bool IsCustomerScopePath(string path)
        => Normalize(path).StartsWith(CustomerScopeRoutePrefix, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Reads the API key presented by a request
    /// </summary>
    /// <param name="headers">Request headers</param>
    /// <returns>The presented API key, or null when the request carries none</returns>
    /// <remarks>
    /// The bearer token wins over the dedicated header, so a client can use the same header for this API
    /// and for other bearer secured services. Shared by the authentication handler and the rate limiter,
    /// which must agree on the credential a request presented.
    /// </remarks>
    public static string GetTokenFromRequest(IHeaderDictionary headers)
    {
        if (headers == null)
            return null;

        if (headers.TryGetValue(AuthorizationHeaderName, out var authorization))
        {
            var value = authorization.ToString();
            if (value.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
                return value[BearerPrefix.Length..].Trim();
        }

        if (headers.TryGetValue(ApiKeyHeaderName, out var apiKey))
            return apiKey.ToString().Trim();

        return null;
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
