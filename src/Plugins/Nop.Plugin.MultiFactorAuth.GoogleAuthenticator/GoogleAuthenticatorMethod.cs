using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Components;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator;

/// <summary>
/// Represents method for the multi-factor authentication with Google Authenticator
/// </summary>
public class GoogleAuthenticatorMethod : BasePlugin, IMultiFactorAuthenticationMethod
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly ILocalizationService _localizationService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlHelperFactory _urlHelperFactory;

    #endregion

    #region Ctor

    public GoogleAuthenticatorMethod(IActionContextAccessor actionContextAccessor,
        ILocalizationService localizationService,
        ISettingService settingService,
        IStoreContext storeContext,
        IUrlHelperFactory urlHelperFactory)
    {
        _actionContextAccessor = actionContextAccessor;
        _localizationService = localizationService;
        _settingService = settingService;
        _storeContext = storeContext;
        _urlHelperFactory = urlHelperFactory;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(GoogleAuthenticatorDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Gets a type of a view component for displaying plugin in public store
    /// </summary>
    /// <returns>View component name</returns>
    public Type GetPublicViewComponent()
    {
        return typeof(GAAuthenticationViewComponent);
    }

    /// <summary>
    /// Gets a type of a view component for displaying verification page
    /// </summary>
    /// <returns>View component name</returns>
    public Type GetVerificationViewComponent()
    {
        return typeof(GAVerificationViewComponent);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        //settings
        await _settingService.SaveSettingAsync(new GoogleAuthenticatorSettings
        {
            BusinessPrefix = store.Name,
            QRPixelsPerModule = GoogleAuthenticatorDefaults.DefaultQRPixelsPerModule
        });

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            //admin config 
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.BusinessPrefix"] = "Business prefix",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.BusinessPrefix.Hint"] = "Provide your business prefix so users can differentiate the account information for your store in the GoogleAuthenticator app.",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.QRPixelsPerModule"] = "QRPixelsPerModule",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.QRPixelsPerModule.Hint"] = "Sets the number of pixels per unit. The module is one square in the QR code. By default is 3 for a 171x171 pixel image.",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Instructions"] = "To use Google Authenticator, the app is first installed on a smartphone. The plugin provides a shared secret key to the user over a secure channel, to be stored in the Google Authenticator app. This secret key will be used for all future logins to the site.",

            //db fields
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Customer"] = "Customer",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.SecretKey"] = "Secret key",

            //customer config
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Customer.VerificationToken"] = "Google Authenticator Code",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Customer.ManualSetupCode"] = "Manual entry setup code",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Customer.SendCode"] = "Confirm",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Customer.Instruction"] = "Please download the app Google Authenticator to scan this QR code. If you already have a verified account with Google Authenticator, then you can change the parameters of your account by registering on this page. Your data in the system will be updated. Since the user is authenticated by email, make sure it is specified for your account.",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Customer.InstructionManual"] = "You can not scan code? You can add the entry manually, please provide the following details to the application on your phone.",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Customer.Account"] = "Account: ",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Customer.TypeKey"] = "Time based : Yes",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Customer.Key"] = "Key: ",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.MultiFactorAuthenticationMethodDescription"] = "Google Authenticator is a software-based authenticator by Google that implements two-step verification services, for authenticating users",

            //validators
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Required"] = "Field cannot be empty. Enter the code from the Google Authenticator app no your mobile phone.",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Fields.Code.Wrong"] = "Field must be 6 digits. Enter the code from the Google Authenticator app no your mobile phone.",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Token.Unsuccessful"] = "Invalid token or its lifetime has expired.",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Token.Successful"] = "Configuration of Google Authenticator for current customer saved successful.",
            ["Plugins.MultiFactorAuth.GoogleAuthenticator.Record.Notfound"] = "Failed to match user credentials to active authentication provider settings record."
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<GoogleAuthenticatorSettings>();

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.MultiFactorAuth.GoogleAuthenticator");

        await base.UninstallAsync();
    }

    /// <summary>
    /// Gets a multi-factor authentication method description that will be displayed on customer info pages in the public store
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task<string> GetDescriptionAsync()
    {
        return await _localizationService
            .GetResourceAsync(
                "Plugins.MultiFactorAuth.GoogleAuthenticator.MultiFactorAuthenticationMethodDescription");
    }

    #endregion

    #region Properies

    public MultiFactorAuthenticationType Type => MultiFactorAuthenticationType.ApplicationVerification;

    #endregion
}