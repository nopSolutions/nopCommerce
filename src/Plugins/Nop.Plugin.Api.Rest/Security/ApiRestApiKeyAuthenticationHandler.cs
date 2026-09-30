using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Nop.Plugin.Api.Rest.Security;

/// <summary>
/// Validates the API key presented in the Authorization (Bearer) or X-Api-Key request header
/// </summary>
public class ApiRestApiKeyAuthenticationHandler : AuthenticationHandler<ApiRestApiKeyAuthenticationOptions>
{
    #region Ctor

    public ApiRestApiKeyAuthenticationHandler(IOptionsMonitor<ApiRestApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle an authentication request
    /// </summary>
    /// <returns>A task that represents the asynchronous authentication operation</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var settings = Context.RequestServices.GetService(typeof(ApiRestSettings)) as ApiRestSettings;
        var token = GetTokenFromRequest();

        //no result rather than a failure, so the caller decides how to treat an anonymous request
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(settings?.ApiKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        return Task.FromResult(KeysMatch(settings.ApiKey, token)
            ? AuthenticateResult.Success(CreateTicket())
            : AuthenticateResult.NoResult());
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create an authenticated ticket
    /// </summary>
    /// <returns>The authentication ticket</returns>
    protected virtual AuthenticationTicket CreateTicket()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, ApiRestDefaults.SecuritySchemeId) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);

        return new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
    }

    /// <summary>
    /// Get the API key from the Authorization (Bearer) or X-Api-Key request headers
    /// </summary>
    /// <returns>The API key, if present</returns>
    protected virtual string GetTokenFromRequest()
    {
        //Authorization wins, so a client can use the same header for this API and for other bearer secured services
        if (Request.Headers.TryGetValue("Authorization", out var authorization))
        {
            var value = authorization.ToString();
            if (value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return value["Bearer ".Length..].Trim();
        }

        if (Request.Headers.TryGetValue(ApiRestDefaults.ApiKeyHeaderName, out var apiKey))
            return apiKey.ToString().Trim();

        return null;
    }

    /// <summary>
    /// Compare the expected and provided keys without leaking the position of the first mismatch
    /// </summary>
    /// <param name="expected">Expected key</param>
    /// <param name="provided">Provided key</param>
    /// <returns>True if the keys match</returns>
    protected static bool KeysMatch(string expected, string provided)
    {
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var providedBytes = Encoding.UTF8.GetBytes(provided);

        return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }

    #endregion
}