using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Api.Rest.Security;

/// <summary>
/// Issues and validates the bearer tokens handed out by the plugin
/// </summary>
/// <remarks>
/// One token format serves both scopes. The <c>nop:credential</c> claim records which one it is, so a
/// caller holding an administrator token is refused by the customer scoped endpoints and vice versa,
/// without either endpoint needing a second parsing path. Signed with the plugin API key, so no second
/// secret has to be provisioned or rotated. The trade off is that regenerating the API key invalidates
/// every token already issued, which is the behaviour a merchant expects after rotating a credential
/// anyway.
/// </remarks>
public static class ApiTokenFactory
{
    /// <summary>
    /// The shortest API key that may sign a token
    /// </summary>
    /// <remarks>
    /// HS256 needs at least 256 bits of key material. A short key configured by hand would otherwise
    /// fail inside the crypto layer, far from the request that caused it.
    /// </remarks>
    public const int MINIMUM_SIGNING_KEY_LENGTH = 32;

    /// <summary>
    /// Checks whether an API key is long enough to sign a token
    /// </summary>
    /// <param name="apiKey">The configured API key</param>
    /// <returns>True when the key can be used as a signing secret</returns>
    public static bool CanSignToken(string apiKey)
        => !string.IsNullOrEmpty(apiKey) && apiKey.Length >= MINIMUM_SIGNING_KEY_LENGTH;

    /// <summary>
    /// Issues a token for the passed credential type
    /// </summary>
    /// <param name="customer">The customer the token is issued to</param>
    /// <param name="apiKey">The configured API key, used as the signing secret</param>
    /// <param name="lifetime">How long the token stays valid</param>
    /// <param name="credentialType">The scope the token grants, see <see cref="ApiRestDefaults.CredentialTypeApiKey"/></param>
    /// <returns>The signed token</returns>
    /// <remarks>
    /// A customer token carries the customer identifier, because that is what the customer scoped
    /// endpoints resolve their data from. An administrator token deliberately does not: it grants
    /// everything rather than one customer's data, and carrying an identifier would make it look like
    /// a customer token to the rate limiter and let an admin token share a shopper's bucket.
    /// </remarks>
    public static string CreateToken(Customer customer, string apiKey, TimeSpan lifetime, string credentialType)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (!CanSignToken(apiKey))
            throw new ArgumentException(
                $"The API key must be at least {MINIMUM_SIGNING_KEY_LENGTH} characters long to sign a token.",
                nameof(apiKey));

        if (lifetime <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(lifetime), "A token must be issued with a positive lifetime.");

        var isCustomerToken = !string.Equals(credentialType, ApiRestDefaults.CredentialTypeApiKey, StringComparison.Ordinal);
        var now = DateTime.UtcNow;

        //the same claim set the official nopCommerce Web API issues: the customer identifier, the
        //customer GUID as the name identifier claim, and the email address. Keeping them identical
        //means a token issued here is shaped like one issued by the official API, so an integration
        //written against one reads the other.
        var claims = new List<Claim>
        {
            new(ApiRestDefaults.CredentialTypeClaim, credentialType)
        };

        if (isCustomerToken)
            claims.Insert(0, new Claim(ApiRestDefaults.CustomerIdClaim, customer.Id.ToString()));

        claims.Add(new Claim(ClaimTypes.NameIdentifier, customer.CustomerGuid.ToString()));

        if (!string.IsNullOrWhiteSpace(customer.Email))
            claims.Add(new Claim(ClaimTypes.Email, customer.Email));

        var credentials = new SigningCredentials(GetSigningKey(apiKey), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            notBefore: now,
            expires: now.Add(lifetime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates a presented token and reads the scope it was issued for
    /// </summary>
    /// <param name="token">The presented token</param>
    /// <param name="apiKey">The configured API key, used as the signing secret</param>
    /// <param name="credentialType">The scope the token grants</param>
    /// <param name="customerId">The customer a customer token was issued to, 0 for an administrator token</param>
    /// <returns>True when the token is valid, unexpired and signed with the configured key</returns>
    public static bool TryValidateToken(string token, string apiKey, out string credentialType, out int customerId)
    {
        credentialType = null;
        customerId = 0;

        if (string.IsNullOrWhiteSpace(token) || !CanSignToken(apiKey))
            return false;

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetSigningKey(apiKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            //the lifetime is enforced here rather than by the parameters, so a token that has just
            //expired is refused instead of being reported as valid
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ApiRestDefaults.CustomerIdClaim
        };

        try
        {
            var handler = new JsonWebTokenHandler();

            if (!handler.CanReadToken(token))
                return false;

            var result = handler.ValidateTokenAsync(token, parameters).GetAwaiter().GetResult();

            if (!result.IsValid)
                return false;

            credentialType = result.Claims.TryGetValue(ApiRestDefaults.CredentialTypeClaim, out var rawType)
                ? rawType?.ToString()
                : null;

            //a token that does not name a scope is not one this plugin issued, so it is not accepted
            if (string.IsNullOrWhiteSpace(credentialType))
                return false;

            var isCustomerToken = !string.Equals(credentialType, ApiRestDefaults.CredentialTypeApiKey, StringComparison.Ordinal);

            if (!isCustomerToken)
                return true;

            var rawId = result.Claims.TryGetValue(ApiRestDefaults.CustomerIdClaim, out var value)
                ? value?.ToString()
                : null;

            return int.TryParse(rawId, out customerId) && customerId > 0;
        }
        catch (Exception)
        {
            //a malformed or wrongly signed token is simply not a valid credential, and must not surface
            //as a server error to the caller
            credentialType = null;
            customerId = 0;
            return false;
        }
    }

    /// <summary>
    /// Turns the API key into the symmetric signing key
    /// </summary>
    /// <param name="apiKey">The configured API key</param>
    /// <returns>The signing key</returns>
    internal static SymmetricSecurityKey GetSigningKey(string apiKey)
        => new(Encoding.UTF8.GetBytes(apiKey));
}