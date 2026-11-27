using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Plugin.ExternalAuth.Facebook.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.Facebook.Admin.Controllers;

public class FacebookAuthenticationAdminController : BasePluginAdminController
{
    #region Fields

    protected readonly FacebookExternalAuthSettings _facebookExternalAuthSettings;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IOptionsMonitorCache<FacebookOptions> _optionsCache;
    protected readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public FacebookAuthenticationAdminController(FacebookExternalAuthSettings facebookExternalAuthSettings,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IOptionsMonitorCache<FacebookOptions> optionsCache,
        ISettingService settingService)
    {
        _facebookExternalAuthSettings = facebookExternalAuthSettings;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _optionsCache = optionsCache;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS)]
    public IActionResult Configure()
    {
        var model = new ConfigurationModel
        {
            ClientId = _facebookExternalAuthSettings.ClientKeyIdentifier,
            ClientSecret = _facebookExternalAuthSettings.ClientSecret
        };

        return View("~/Plugins/ExternalAuth.Facebook/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_EXTERNAL_AUTHENTICATION_METHODS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return Configure();

        //save settings
        _facebookExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
        _facebookExternalAuthSettings.ClientSecret = model.ClientSecret;
        await _settingService.SaveSettingAsync(_facebookExternalAuthSettings);

        //clear Facebook authentication options cache
        _optionsCache.TryRemove(FacebookDefaults.AuthenticationScheme);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    #endregion
}