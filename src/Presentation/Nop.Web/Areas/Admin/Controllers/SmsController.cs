using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
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
    protected readonly OtpSettings _otpSettings;    

    #endregion

    #region Ctor

    public SmsController(ISettingService settingService,
        ISmsModelFactory taxModelFactory,
        ISmsPluginManager smsPluginManager,
        OtpSettings otpSettings)
    {
        _settingService = settingService;
        _smsModelFactory = taxModelFactory;
        _smsPluginManager = smsPluginManager;
        _otpSettings = otpSettings;
    }

    #endregion

    #region Methods

    public virtual IActionResult List()
    {
        return RedirectToAction("Providers");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Providers()
    {
        //prepare model
        var model = await _smsModelFactory.PrepareSmsProviderSearchModelAsync(new SmsProviderSearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Providers(SmsProviderSearchModel searchModel)
    {
        //prepare model
        var model = await _smsModelFactory.PrepareSmsProviderListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> MarkAsPrimaryProvider(string systemName)
    {
        if (string.IsNullOrEmpty(systemName))
            return RedirectToAction("Providers");

        var taxProvider = await _smsPluginManager.LoadPluginBySystemNameAsync(systemName);
        if (taxProvider == null)
            return RedirectToAction("Providers");

        _otpSettings.ActiveSmsProviderSystemName = systemName;
        await _settingService.SaveSettingAsync(_otpSettings);

        return RedirectToAction("Providers");
    }

    #endregion
}
