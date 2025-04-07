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
public class AmazonPayCustomerService(AmazonPayApiService amazonPayApiService,
        AmazonPaySettings amazonPaySettings,
        IAuthenticationService authenticationService,
        IExternalAuthenticationService externalAuthenticationService,
        ICustomerService customerService,
        ILogger logger,
        IWebHelper webHelper,
        IWorkContext workContext)
{
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

            var customer = await workContext.GetCurrentCustomerAsync();
            if (await customerService.IsGuestAsync(customer))
            {
                if (await customerService.GetCustomerByEmailAsync(buyerEmail) is not null)
                    return true;

                await externalAuthenticationService.AuthenticateAsync(authenticationParameters);
            }
            else
            {
                var associatedCustomer =
                    await externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                        new ExternalAuthenticationParameters
                        {
                            ExternalIdentifier = buyerId,
                            ProviderSystemName = AmazonPayDefaults.PluginSystemName
                        });

                if (associatedCustomer is null)
                    await externalAuthenticationService.AssociateExternalAccountWithUserAsync(customer, authenticationParameters);
            }

            return false;
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await logger.ErrorAsync(logMessage, exception, await workContext.GetCurrentCustomerAsync());

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
                signInReturnUrl: amazonPayApiService.GetUrl(AmazonPayDefaults.SignInHandlerRouteName),
                storeId: amazonPaySettings.StoreId,
                signInScopes: scopes
            );

            var signature = await amazonPayApiService.GenerateButtonSignatureAsync(request);
            var payload = request.ToJson();

            var model = new SignInModel
            {
                LedgerCurrency = amazonPayApiService.LedgerCurrency?.ToString(),
                ButtonColor = amazonPaySettings.ButtonColor.ToString(),
                AmazonPayScript = amazonPayApiService.AmazonPayScriptUrl,
                Payload = payload,
                Signature = signature
            };

            return model;
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await logger.ErrorAsync(logMessage, exception, await workContext.GetCurrentCustomerAsync());

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
            var buyerToken = webHelper.QueryString<string>(AmazonPayDefaults.BuyerTokenQueryParamName);

            var result = await amazonPayApiService.PerformRequestAsync(client => client.GetBuyer(buyerToken));

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = AmazonPayDefaults.PluginSystemName,
                AccessToken = buyerToken,
                Email = result.Email,
                ExternalIdentifier = result.BuyerId,
                ExternalDisplayIdentifier = result.Name
            };

            return await externalAuthenticationService.AuthenticateAsync(authenticationParameters);
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await logger.ErrorAsync(logMessage, exception, await workContext.GetCurrentCustomerAsync());

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
            var customer = await workContext.GetCurrentCustomerAsync();
            if (await customerService.IsGuestAsync(customer))
                return;

            var associatedCustomer =
                await externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                    new ExternalAuthenticationParameters
                    {
                        ExternalIdentifier = buyerId,
                        ProviderSystemName = AmazonPayDefaults.PluginSystemName
                    });

            if (associatedCustomer is not null && associatedCustomer.Id == customer.Id)
                await authenticationService.SignOutAsync();
        }
        catch (Exception exception)
        {
            var logMessage = $"{AmazonPayDefaults.PluginSystemName} error:{System.Environment.NewLine}{exception.Message}";
            await logger.ErrorAsync(logMessage, exception, await workContext.GetCurrentCustomerAsync());
        }
    }

    #endregion
}