using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AzureBlob.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.AzureBlob.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class AzureBlobController : BasePluginController
{
    #region Fields

    private readonly AzureBlobSettings _azureBlobSettings;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor

    public AzureBlobController(AzureBlobSettings azureBlobSettings,
        ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService)
    {
        _azureBlobSettings = azureBlobSettings;
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
            AppendContainerName = _azureBlobSettings.AppendContainerName,
            ConnectionString = _azureBlobSettings.ConnectionString,
            ContainerName = _azureBlobSettings.ContainerName,
            Enabled = _azureBlobSettings.Enabled,
            EndPoint = _azureBlobSettings.EndPoint,
        };

        return View("~/Plugins/Misc.AzureBlob/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        _azureBlobSettings.AppendContainerName = model.AppendContainerName;
        _azureBlobSettings.ConnectionString = model.ConnectionString;
        _azureBlobSettings.ContainerName = model.ContainerName;
        _azureBlobSettings.Enabled = model.Enabled;
        _azureBlobSettings.EndPoint = model.EndPoint;

        await _settingService.SaveSettingAsync(_azureBlobSettings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    #endregion
}