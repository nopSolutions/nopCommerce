using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.PunchOut;

/// <summary>
/// Represents PunchOut plugin
/// </summary>
public class PunchOutMethod : BasePlugin, IMiscPlugin
{
    #region Fields

    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public PunchOutMethod(ICustomerService customerService,
        ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService)
    {
        _customerService = customerService;
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(PunchOutDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new PunchOutSettings
        {
            IsActive = false,
            TimeToExpire = PunchOutDefaults.TimeToExpireSession,
            RestrictedCustomerRoleIds = [(await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName)).Id]
        });

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Misc.PunchOut.Configuration"] = "Configuration",
            ["Plugins.Misc.PunchOut.Configuration.Common"] = "Common settings",

            ["Plugins.Misc.PunchOut.Configuration.IsActive"] = "Is active",
            ["Plugins.Misc.PunchOut.Configuration.IsActive.Hint"] = "Enable or disable the plugin.",
            ["Plugins.Misc.PunchOut.Configuration.CustomerRoles"] = "Restricted customer roles",
            ["Plugins.Misc.PunchOut.Configuration.CustomerRoles.Hint"] = "Select customer roles that will have't access to the PunchOut feature. If no role is selected, all customers will have access.",
            ["Plugins.Misc.PunchOut.Configuration.CustomerRoles.NoRoles"] = "No customer roles found",
            ["Plugins.Misc.PunchOut.Configuration.TimeToExpire"] = "Time to expire",
            ["Plugins.Misc.PunchOut.Configuration.TimeToExpire.Hint"] = "The time in hours after which the PunchOut session will be expired.",
            ["Plugins.Misc.PunchOut.SessionExpired"] = "Your PunchOut session has expired.",
            ["Plugins.Misc.PunchOut.ServiceUnavailable"] = "The PunchOut service is currently unavailable.",

            ["Plugins.Misc.PunchOut.Log"] = "Log",
            ["Plugins.Misc.PunchOut.Log.Hint"] = "View log entry details.",
            ["Plugins.Misc.PunchOut.Log.Search.CreatedFrom"] = "Created from",
            ["Plugins.Misc.PunchOut.Log.Search.CreatedFrom.Hint"] = "The creation from date for the search.",
            ["Plugins.Misc.PunchOut.Log.Search.CreatedTo"] = "Created to",
            ["Plugins.Misc.PunchOut.Log.Search.CreatedTo.Hint"] = "The creation to date for the search.",
            ["Plugins.Misc.PunchOut.Log.BackToList"] = "back to log",
            ["Plugins.Misc.PunchOut.Log.CreatedDate"] = "Created date",
            ["Plugins.Misc.PunchOut.Log.SessionId"] = "Session ID",
            ["Plugins.Misc.PunchOut.Log.MessageType"] = "Message type",
            ["Plugins.Misc.PunchOut.Log.Deleted"] = "The log entry has been deleted successfully.",
            ["Plugins.Misc.PunchOut.Log.Direction"] = "Direction",
            ["Plugins.Misc.PunchOut.Log.RawXml"] = "Raw XML",
            ["Plugins.Misc.PunchOut.Log.Url"] = "URL",
            ["Plugins.Misc.PunchOut.Log.Identity"] = "Identity",
            ["Plugins.Misc.PunchOut.Log.Error"] = "Error",

            ["Plugins.Misc.PunchOut.Identity"] = "Identity",
            ["Plugins.Misc.PunchOut.Identity.Identity"] = "Identity",
            ["Plugins.Misc.PunchOut.Identity.Identity.Hint"] = "The identity for the Sender feature.",
            ["Plugins.Misc.PunchOut.Identity.SharedSecret"] = "Shared secret",
            ["Plugins.Misc.PunchOut.Identity.SharedSecret.Hint"] = "The shared secret for the Sender feature.",
            ["Plugins.Misc.PunchOut.Identity.SharedSecretHash"] = "Shared secret hash",
            ["Plugins.Misc.PunchOut.Identity.Search.Identity"] = "Identity",
            ["Plugins.Misc.PunchOut.Identity.Search.Identity.Hint"] = "The identity for the search.",

            ["Plugins.Misc.PunchOut.Session"] = "Session",
            ["Plugins.Misc.PunchOut.Session.SessionId"] = "Session ID",
            ["Plugins.Misc.PunchOut.Session.BuyerCookie"] = "Buyer Cookie",
            ["Plugins.Misc.PunchOut.Session.IsActive"] = "Is Active",
            ["Plugins.Misc.PunchOut.Session.CustomerEmail"] = "Customer Email",
            ["Plugins.Misc.PunchOut.Session.Store"] = "Store",
            ["Plugins.Misc.PunchOut.Session.CreatedOnUtc"] = "Created On",
            ["Plugins.Misc.PunchOut.Session.CloseSession"] = "Close Session",
            ["Plugins.Misc.PunchOut.Session.CloseSessionError"] = "An error occurred while closing the PunchOut session.",

            ["Plugins.Misc.PunchOut.Button.SubmitForApproval"] = "Submit for Approval",
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        await _settingService.DeleteSettingAsync<PunchOutSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.PunchOut");

        await base.UninstallAsync();
    }

    #endregion    
}
