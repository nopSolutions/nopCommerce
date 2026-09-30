using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Api.Rest.Security;

/// <summary>
/// Validates the credential presented in the Authorization (Bearer) or X-Api-Key request header
/// </summary>
/// <remarks>
/// Two credentials are accepted, and the request is told which one it presented: the shared API key,
/// which carries admin level access, and a customer token, which only identifies the customer it was
/// issued to. Both are handled by one scheme so the pipeline, the rate limiter and the Swagger document
/// all see a single authentication result.
/// </remarks>
public class ApiRestApiKeyAuthenticationHandler : AuthenticationHandler<ApiRestApiKeyAuthenticationOptions>
{
    #region Ctor

    public ApiRestApiKeyAuthenticationHandler(IOptionsMonitor<ApiRestApiKeyAuthenticationOptions> options,
        ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

    #endregion

    #region Methods

    /// <summary>
    /// Handle an authentication request
    /// </summary>
    /// <returns>A task that represents the asynchronous authentication operation</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var settings = Context.RequestServices.GetService(typeof(ApiRestSettings)) as ApiRestSettings;
        var presented = GetTokenFromRequest();

        //no result rather than a failure, so the caller decides how to treat an anonymous request
        if (string.IsNullOrWhiteSpace(presented) || string.IsNullOrWhiteSpace(settings?.ApiKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (KeysMatch(settings.ApiKey, presented))
            return Task.FromResult(AuthenticateResult.Success(CreateApiKeyTicket()));

        //not the API key, so it may be a token issued to a customer
        if (CustomerTokenFactory.TryValidateToken(presented, settings.ApiKey, out var customerId))
            return Task.FromResult(AuthenticateResult.Success(CreateCustomerTicket(customerId)));

        return Task.FromResult(AuthenticateResult.NoResult());
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create an authenticated ticket for the shared API key
    /// </summary>
    /// <returns>The authentication ticket</returns>
    protected virtual AuthenticationTicket CreateApiKeyTicket()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, ApiRestDefaults.CredentialTypeApiKey),
            new Claim(ApiRestDefaults.CredentialTypeClaim, ApiRestDefaults.CredentialTypeApiKey)
        };

        return CreateTicket(claims);
    }

    /// <summary>
    /// Create an authenticated ticket for a customer token
    /// </summary>
    /// <param name="customerId">The customer the token was issued to</param>
    /// <returns>The authentication ticket</returns>
    protected virtual AuthenticationTicket CreateCustomerTicket(int customerId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, ApiRestDefaults.CredentialTypeCustomerToken),
            new Claim(ApiRestDefaults.CredentialTypeClaim, ApiRestDefaults.CredentialTypeCustomerToken),
            new Claim(ApiRestDefaults.CustomerIdClaim, customerId.ToString())
        };

        return CreateTicket(claims);
    }

    /// <summary>
    /// Create an authenticated ticket carrying the passed claims
    /// </summary>
    /// <param name="claims">The claims identifying the credential</param>
    /// <returns>The authentication ticket</returns>
    protected virtual AuthenticationTicket CreateTicket(Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, Scheme.Name);

        return new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
    }

    /// <summary>
    /// Get the credential from the Authorization (Bearer) or X-Api-Key request headers
    /// </summary>
    /// <returns>The presented credential, if any</returns>
    protected virtual string GetTokenFromRequest()
    {
        return ApiRestDefaults.GetTokenFromRequest(Request.Headers);
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
