using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.CloudflareImages.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.CloudflareImages.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class CloudflareImagesController : BasePluginController
{
    #region Fields

    private readonly CloudflareImagesSettings _cloudflareImagesSettings;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public CloudflareImagesController(CloudflareImagesSettings cloudflareImagesSettings,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService)
    {
        _cloudflareImagesSettings = cloudflareImagesSettings;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
        var model = new ConfigurationModel
        {
            Enabled = _cloudflareImagesSettings.Enabled,
            DeliveryUrl = _cloudflareImagesSettings.DeliveryUrl,
            AccessToken = _cloudflareImagesSettings.AccessToken,
            AccountId = _cloudflareImagesSettings.AccountId,
            RequestTimeout = _cloudflareImagesSettings.RequestTimeout
        };

        return View("~/Plugins/Misc.CloudflareImages/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        _cloudflareImagesSettings.Enabled = model.Enabled;
        _cloudflareImagesSettings.DeliveryUrl = model.DeliveryUrl;
        _cloudflareImagesSettings.AccessToken = model.AccessToken;
        _cloudflareImagesSettings.AccountId = model.AccountId;
        _cloudflareImagesSettings.RequestTimeout = model.RequestTimeout;

        await _settingService.SaveSettingAsync(_cloudflareImagesSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    #endregion
}