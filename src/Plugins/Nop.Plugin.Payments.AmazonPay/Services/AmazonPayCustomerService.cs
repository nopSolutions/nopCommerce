using Amazon.Pay.API.WebStore.Buyer;
using Amazon.Pay.API.WebStore.Types;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.AmazonPay.Models;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Customers;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.AmazonPay.Services;

/// <summary>
/// Represents the service for customer related methods
/// </summary>
public class AmazonPayCustomerService
{
    #region Fields

    private readonly AmazonPayApiService _amazonPayApiService;
    private readonly AmazonPaySettings _amazonPaySettings;
    private readonly IAuthenticationService _authenticationService;
    private readonly IExternalAuthenticationService _externalAuthenticationService;
    private readonly ICustomerService _customerService;
    private readonly ILogger _logger;
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AmazonPayCustomerService(AmazonPayApiService amazonPayApiService,
        AmazonPaySettings amazonPaySettings,
        IAuthenticationService authenticationService,
        IExternalAuthenticationService externalAuthenticationService,
        ICustomerService customerService,
        ILogger logger,
        IWebHelper webHelper,
        IWorkContext workContext)
    {
        _amazonPayApiService = amazonPayApiService;
        _amazonPaySettings = amazonPaySettings;
        _authenticationService = authenticationService;
        _externalAuthenticationService = externalAuthenticationService;
        _customerService = customerService;
        _logger = logger;
        _webHelper = webHelper;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Associate or create customer to/by Amazon pay account
    /// </summary>
    /// <param name="buyerId">External account identifier</param>
    /// <param name="buyerEmail">External account email</param>
    /// <param name="buyerName">External account name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains a value whether to login customer
    /// </returns>
    public async Task<bool> AssociateOrCreateCustomerAsync(string buyerId, string buyerEmail, string buyerName)
    {
        try
        {
            if (string.IsNullOrEmpty(buyerId) || string.IsNullOrEmpty(buyerEmail))
                return false;

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = AmazonPayDefaults.PluginSystemName,
                Email = buyerEmail,
                ExternalIdentifier = buyerId,
                ExternalDisplayIdentifier = buyerName
            };

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
            {
                if (await _customerService.GetCustomerByEmailAsync(buyerEmail) is not null)
                    return true;

                await _externalAuthenticationService.AuthenticateAsync(authenticationParameters);
            }
            else
            {
                var associatedCustomer =
                    await _externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                        new ExternalAuthenticationParameters
                        {
                            ExternalIdentifier = buyerId,
                            ProviderSystemName = AmazonPayDefaults.PluginSystemName
                        });

                if (associatedCustomer is null)
                    await _externalAuthenticationService.AssociateExternalAccountWithUserAsync(customer, authenticationParameters);
            }

            return false;
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

            return false;
        }
    }

    /// <summary>
    /// Gets the sign in model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sign in model
    /// </returns>
    public async Task<SignInModel> GetSignInModelAsync()
    {
        try
        {
            SignInScope[] scopes = { SignInScope.Name, SignInScope.Email, SignInScope.PhoneNumber };

            var request = new SignInRequest
            (
                signInReturnUrl: _amazonPayApiService.GetUrl(AmazonPayDefaults.SignInHandlerRouteName),
                storeId: _amazonPaySettings.StoreId,
                signInScopes: scopes
            );

            var signature = await _amazonPayApiService.GenerateButtonSignatureAsync(request);
            var payload = request.ToJson();

            var model = new SignInModel
            {
                LedgerCurrency = _amazonPayApiService.LedgerCurrency?.ToString(),
                ButtonColor = _amazonPaySettings.ButtonColor.ToString(),
                AmazonPayScript = _amazonPayApiService.AmazonPayScriptUrl,
                Payload = payload,
                Signature = signature
            };

            return model;
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

            return null;
        }
    }

    /// <summary>
    /// Sign in customer by Amazon Pay account
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the IActionResult
    /// </returns>
    public async Task<IActionResult> SignInAsync()
    {
        try
        {
            // the token as retrieved from the URL
            var buyerToken = _webHelper.QueryString<string>(AmazonPayDefaults.BuyerTokenQueryParamName);

            var result = await _amazonPayApiService.PerformRequestAsync(client => client.GetBuyer(buyerToken));

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = AmazonPayDefaults.PluginSystemName,
                AccessToken = buyerToken,
                Email = result.Email,
                ExternalIdentifier = result.BuyerId,
                ExternalDisplayIdentifier = result.Name
            };

            return await _externalAuthenticationService.AuthenticateAsync(authenticationParameters);
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());

            return new RedirectToRouteResult("Login");
        }
    }

    /// <summary>
    /// Sign out current customer
    /// </summary>
    /// <param name="buyerId">External account identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task SignOutAsync(string buyerId)
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer))
                return;

            var associatedCustomer =
                await _externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                    new ExternalAuthenticationParameters
                    {
                        ExternalIdentifier = buyerId,
                        ProviderSystemName = AmazonPayDefaults.PluginSystemName
                    });

            if (associatedCustomer is not null && associatedCustomer.Id == customer.Id)
                await _authenticationService.SignOutAsync();
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await _logger.ErrorAsync(logMessage, exception, await _workContext.GetCurrentCustomerAsync());
        }
    }

    #endregion
}