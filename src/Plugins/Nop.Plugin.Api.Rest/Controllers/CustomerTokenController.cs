using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Rest.Models;
using Nop.Plugin.Api.Rest.Models.Requests;
using Nop.Plugin.Api.Rest.Security;
using Nop.Services.Customers;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Rest.Controllers;

/// <summary>
/// Issues a customer token, which identifies the store customer a request acts on behalf of
/// </summary>
/// <remarks>
/// The counterpart of <see cref="ApiKeyController"/>. The shared API key grants admin level access to
/// every record; a customer token grants access to the data of the single customer it was issued to, and
/// is what the <c>/api/rest/customer/me</c> endpoints require.
/// </remarks>
[ApiController]
[Route("api/rest/customer/token")]
public class CustomerTokenController : ControllerBase
{
    #region Fields

    private readonly ICustomerRegistrationService _customerRegistrationService;
    private readonly ICustomerService _customerService;
    private readonly CustomerSettings _customerSettings;
    private readonly ILocalizationService _localizationService;
    private readonly ApiRestSettings _settings;

    #endregion

    #region Ctor

    public CustomerTokenController(ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        CustomerSettings customerSettings,
        ILocalizationService localizationService,
        ApiRestSettings settings)
    {
        _customerRegistrationService = customerRegistrationService;
        _customerService = customerService;
        _customerSettings = customerSettings;
        _localizationService = localizationService;
        _settings = settings;
    }

    #endregion

    #region Methods

/// <summary>
        /// Exchanges store credentials for a bearer token
        /// </summary>
        /// <param name="request">The customer credentials</param>
        /// <returns>The token to send back as a bearer token</returns>
        [HttpPost]
        public virtual async Task<IActionResult> Token([FromBody] GetTokenRequest request)
    {
        if (request == null)
            return BadRequest(new { error = "A request body with 'email' and 'password' is required." });

        var identifier = ResolveIdentifier(request);

        if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { error = "Both 'email' and 'password' are required." });

        if (string.IsNullOrEmpty(_settings.ApiKey))
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                error = "No API key is configured, so customer tokens cannot be signed. Generate one on the plugin configuration page."
            });

if (!ApiTokenFactory.CanSignToken(_settings.ApiKey))
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    error = $"The configured API key is too short to sign a token. It must be at least {ApiTokenFactory.MINIMUM_SIGNING_KEY_LENGTH} characters. Generate a new one on the plugin configuration page."
                });

        //reuses the store credential validation, which also applies the failed attempt counter and the
        //account lockout configured for the store
        var loginResult = await _customerRegistrationService.ValidateCustomerAsync(identifier, request.Password);
        if (loginResult != CustomerLoginResults.Successful)
            return Unauthorized(new { error = await GetLoginFailureAsync(loginResult) });

        var customer = await FindCustomerAsync(identifier);
        if (customer == null)
            return Unauthorized(new { error = await _localizationService.GetResourceAsync("Account.Login.WrongCredentials") });

        if (customer.Deleted || !customer.Active)
            return Unauthorized(new { error = "This account cannot sign in." });

        //guests have no account to sign in with, and an administrator using this endpoint would receive a
        //token that silently scopes them to their own customer record instead of the admin level of
        //access the shared API key already grants
        if (!await _customerService.IsRegisteredAsync(customer))
            return Forbid();

        var lifetime = _settings.CustomerTokenLifetime;
        var token = ApiTokenFactory.CreateToken(customer, _settings.ApiKey, lifetime,
            ApiRestDefaults.CredentialTypeCustomerToken);

        return Ok(new ApiTokenDto
        {
            Token = token,
            TokenType = ApiRestDefaults.BearerPrefix.TrimEnd(),
            ExpiresInSeconds = (int)lifetime.TotalSeconds,
            CredentialType = ApiRestDefaults.CredentialTypeCustomerToken,
            CustomerId = customer.Id,
            CustomerGuid = customer.CustomerGuid.ToString(),
            Username = customer.Username ?? customer.Email
        });
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Resolves the identifier to authenticate against, following the store convention
    /// </summary>
    /// <param name="request">The credentials</param>
    /// <returns>Username when the store uses usernames, otherwise the email address</returns>
    protected virtual string ResolveIdentifier(GetTokenRequest request)
        => _customerSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(request.Username)
            ? request.Username
            : request.Email;

    /// <summary>
    /// Loads the customer behind a validated identifier
    /// </summary>
    /// <param name="identifier">Validated username or email address</param>
    /// <returns>The customer, or null when not found</returns>
    protected virtual async Task<Customer> FindCustomerAsync(string identifier)
        => _customerSettings.UsernamesEnabled
            ? await _customerService.GetCustomerByUsernameAsync(identifier)
            : await _customerService.GetCustomerByEmailAsync(identifier);

    /// <summary>
    /// Maps a login result to its localized message
    /// </summary>
    /// <param name="loginResult">Result of the credential validation</param>
    /// <returns>The message to return to the caller</returns>
    protected virtual async Task<string> GetLoginFailureAsync(CustomerLoginResults loginResult)
    {
        var resourceName = loginResult switch
        {
            CustomerLoginResults.Deleted => "Account.Login.WrongCredentials.Deleted",
            CustomerLoginResults.NotActive => "Account.Login.WrongCredentials.NotActive",
            CustomerLoginResults.LockedOut => "Account.Login.WrongCredentials.LockedOut",
            //the API has no way to complete an interactive challenge, so it cannot hand out a token
            CustomerLoginResults.MultiFactorAuthenticationRequired =>
                "Account.Login.WrongCredentials.MultiFactorUnsupported",
            _ => "Account.Login.WrongCredentials"
        };

        var resource = await _localizationService.GetResourceAsync(resourceName);

        return string.IsNullOrEmpty(resource) ? "The credentials provided are incorrect" : resource;
    }

    #endregion
}
