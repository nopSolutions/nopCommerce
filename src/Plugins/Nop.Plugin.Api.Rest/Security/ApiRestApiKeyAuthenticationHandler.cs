using System;
using System.Collections.Generic;
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
/// Validates the credential presented in the Authorization (Bearer) or X-Api-Key request header
/// </summary>
/// <remarks>
/// The two headers carry different formats and neither is read as the other: <c>Authorization</c> carries
/// a signed token, <c>X-Api-Key</c> carries the shared API key. Both are handled by one scheme so the
/// pipeline, the rate limiter and the Swagger document all see a single authentication result, and so
/// that result records which scope the caller proved.
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
        var presented = GetCredential();

        //no result rather than a failure, so the caller decides how to treat an anonymous request
        if (presented.Value == null || string.IsNullOrWhiteSpace(settings?.ApiKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        //each slot is validated only as what it is. A token in the API key header, or the raw key in the
        //Authorization header, is refused rather than guessed at, so a caller cannot present the wrong
        //credential by mistake and be silently given the scope of the other one.
        if (presented.Slot == ApiRestDefaults.CredentialSlot.ApiKeyHeader)
        {
            return Task.FromResult(KeysMatch(settings.ApiKey, presented.Value)
                ? AuthenticateResult.Success(CreateTicket(ApiRestDefaults.CredentialTypeApiKey, 0))
                : AuthenticateResult.NoResult());
        }

        if (!ApiTokenFactory.TryValidateToken(presented.Value, settings.ApiKey, out var credentialType, out var customerId))
            return Task.FromResult(AuthenticateResult.NoResult());

        return Task.FromResult(AuthenticateResult.Success(CreateTicket(credentialType, customerId)));
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Create an authenticated ticket recording the scope the credential grants
    /// </summary>
    /// <param name="credentialType">The scope the credential grants</param>
    /// <param name="customerId">The customer a customer token was issued to, 0 when the credential is not one</param>
    /// <returns>The authentication ticket</returns>
    protected virtual AuthenticationTicket CreateTicket(string credentialType, int customerId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, credentialType),
            new(ApiRestDefaults.CredentialTypeClaim, credentialType)
        };

        //only a customer token identifies one customer. The claim is omitted for the shared key and for
        //an administrator token, so the customer scoped endpoints cannot be reached with either
        if (customerId > 0)
            claims.Add(new Claim(ApiRestDefaults.CustomerIdClaim, customerId.ToString()));

        var identity = new ClaimsIdentity(claims, Scheme.Name);

        return new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
    }

    /// <summary>
    /// Get the credential from the request headers, along with the slot it arrived in
    /// </summary>
    /// <returns>The presented credential</returns>
    protected virtual ApiRestDefaults.PresentedCredential GetCredential()
    {
        return ApiRestDefaults.GetCredential(Request.Headers);
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