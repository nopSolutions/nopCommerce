using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Messages;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Sms;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class SmsController : BaseAdminController
{
    #region Fields

    protected readonly ISettingService _settingService;
    protected readonly ISmsModelFactory _smsModelFactory;
    protected readonly ISmsPluginManager _smsPluginManager;
    protected readonly MessagesSettings _messagesSettings;

    #endregion

    #region Ctor

    public SmsController(ISettingService settingService,
        ISmsModelFactory smsModelFactory,
        ISmsPluginManager smsPluginManager,
        MessagesSettings messagesSettings)
    {
        _settingService = settingService;
        _smsModelFactory = smsModelFactory;
        _smsPluginManager = smsPluginManager;
        _messagesSettings = messagesSettings;
    }

    #endregion

    #region Methods

    public virtual IActionResult List()
    {
        return RedirectToAction("Providers");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SMS_SETTINGS)]
    public virtual async Task<IActionResult> Providers()
    {
        //prepare model
        var model = await _smsModelFactory.PrepareSmsProviderSearchModelAsync(new SmsProviderSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SMS_SETTINGS)]
    public virtual async Task<IActionResult> Providers(SmsProviderSearchModel searchModel)
    {
        //prepare model
        var model = await _smsModelFactory.PrepareSmsProviderListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SMS_SETTINGS)]
    public virtual async Task<IActionResult> MarkAsPrimaryProvider(string systemName)
    {
        if (string.IsNullOrEmpty(systemName))
            return RedirectToAction("Providers");

        var smsProvider = await _smsPluginManager.LoadPluginBySystemNameAsync(systemName);
        if (smsProvider == null)
            return RedirectToAction("Providers");

        _messagesSettings.ActiveSmsProviderSystemName = systemName;
        await _settingService.SaveSettingAsync(_messagesSettings);

        return RedirectToAction("Providers");
    }

    #endregion
}
