using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Rest.Models;
using Nop.Plugin.Api.Rest.Models.Requests;
using Nop.Plugin.Api.Rest.Security;
using Nop.Services.Customers;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Rest.Controllers
{
    /// <summary>
    /// Issues a bearer token to an authenticated administrator.
    /// </summary>
    /// <remarks>
    /// This endpoint is the single exemption from the credential check in <c>ApiKeyRateLimitMiddleware</c>:
    /// it is how a caller obtains that credential, so requiring one here would make it unreachable. It is
    /// still rate limited, and it never issues a token without valid administrator credentials.
    /// <para>
    /// It issues a signed token rather than handing back the shared API key, so that
    /// <c>Authorization: Bearer</c> carries exactly one kind of value. The key itself stays available in
    /// the <c>X-Api-Key</c> header for server to server callers that would rather not hold a token.
    /// </para>
    /// </remarks>
    [ApiController]
    [Route("api/rest/token")]
    public class ApiKeyController : ControllerBase
    {
        #region Fields

        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ApiRestSettings _settings;

        #endregion

        #region Ctor

        public ApiKeyController(ApiRestSettings settings,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            ILocalizationService localizationService)
        {
            _settings = settings;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _customerSettings = customerSettings;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Exchanges administrator credentials for a bearer token
        /// </summary>
        /// <param name="request">Administrator credentials</param>
        /// <returns>The token to send back as a bearer token</returns>
        [HttpPost]
        public virtual async Task<IActionResult> Token([FromBody] GetTokenRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "A request body with 'email' and 'password' is required." });

            var identifier = ResolveIdentifier(request);
            if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { error = "Both 'email' and 'password' are required." });

            //reuses the store credential validation, which also applies the failed attempt
            //counter and the account lockout configured for the store
            var loginResult = await _customerRegistrationService.ValidateCustomerAsync(identifier, request.Password);
            if (loginResult != CustomerLoginResults.Successful)
                return Unauthorized(await GetLoginFailureAsync(loginResult));

            var customer = await FindCustomerAsync(identifier);
            if (customer == null)
                return Unauthorized(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));

            if (customer.Deleted || !customer.Active)
                return Unauthorized(new { error = "This account cannot sign in." });

            //strictly the Administrators role. Vendors may sign in to the admin area but must not
            //receive a token that grants catalog write access.
            if (!await _customerService.IsAdminAsync(customer))
                return Forbid();

            if (string.IsNullOrEmpty(_settings.ApiKey))
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    error = "No API key is configured, so tokens cannot be signed. Generate one on the plugin configuration page."
                });

            //the key is also the signing secret, so a key that is too short to sign with would fail
            //inside the crypto layer instead of here
            if (!ApiTokenFactory.CanSignToken(_settings.ApiKey))
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    error = $"The configured API key is too short to sign a token. It must be at least {ApiTokenFactory.MINIMUM_SIGNING_KEY_LENGTH} characters. Generate a new one on the plugin configuration page."
                });

            var lifetime = _settings.AdminTokenLifetime;
            var token = ApiTokenFactory.CreateToken(customer, _settings.ApiKey, lifetime,
                ApiRestDefaults.CredentialTypeApiKey);

            return Ok(new ApiTokenDto
            {
                Token = token,
                TokenType = ApiRestDefaults.BearerPrefix.TrimEnd(),
                ExpiresInSeconds = (int)lifetime.TotalSeconds,
                CredentialType = ApiRestDefaults.CredentialTypeApiKey,
                Username = customer.Username ?? customer.Email,
                //reported from the same rule the middleware enforces, so enabling the read setting is
                //reflected here instead of still advertising GET as public
                SecuredMethods = ApiRestDefaults.GetSecuredMethods(_settings.RequireApiKeyForReads)
            });
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Resolves the identifier to authenticate against, following the store convention
        /// </summary>
        /// <param name="request">Administrator credentials</param>
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
}