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
    /// Gets the name of the authentication scheme that validates the API key
    /// </summary>
    public static string AuthenticationSchemeName => "ApiRestApiKey";

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
    /// Checks whether a path targets the plugin API
    /// </summary>
    /// <param name="path">
    /// Request path or relative API path. A leading slash is tolerated so both forms can be passed.
    /// </param>
    /// <returns>True when the path belongs to the plugin API</returns>
    public static bool IsApiPath(string path)
        => Normalize(path).StartsWith(ApiRoutePrefix, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether a path targets the API key endpoint, which is exempt from the API key checks
    /// because it is the endpoint callers use to obtain that key in the first place
    /// </summary>
    /// <param name="path">Request path or relative API path</param>
    /// <returns>True when the path targets the token endpoint</returns>
    public static bool IsTokenPath(string path)
        => string.Equals(Normalize(path), TokenRoute, StringComparison.OrdinalIgnoreCase);

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
