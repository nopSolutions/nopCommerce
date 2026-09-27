using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.WorldpayHpp.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.WorldpayHpp.Controllers;

[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
[ValidateIpAddress]
[AuthorizeAdmin]
public class WorldpayHppController : BasePluginController
{
    private readonly IOrderService _orderService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly ISettingService _settingService;
    private readonly IPermissionService _permissionService;
    private readonly ILogger _logger;
    private readonly WorldpayHppSettings _settings;

    public WorldpayHppController(IOrderService orderService,
                                 IOrderProcessingService orderProcessingService,
                                 ISettingService settingService,
                                 IPermissionService permissionService,
                                 ILogger logger,
                                 WorldpayHppSettings settings)
    {
        _orderService = orderService;
        _orderProcessingService = orderProcessingService;
        _settingService = settingService;
        _permissionService = permissionService;
        _logger = logger;
        _settings = settings;
    }

  

   


    // Admin Configure
    [HttpGet]
    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS))
            return AccessDeniedView();

        // Render settings form
        var model = new Models.ConfigurationModel
        {
            EntityId = _settings.EntityId,
            Username = _settings.Username,
            Password = _settings.Password,
            Environment = _settings.Environment,
            NarrativeLine1 = _settings.NarrativeLine1,
            TransactionReferencePrefix = _settings.TransactionReferencePrefix,
                        
            //UseIframe = _settings.UseIframe,
            SuccessUrl = _settings.SuccessUrl,
            PendingUrl = _settings.PendingUrl,
            FailureUrl = _settings.FailureUrl,
            ErrorUrl = _settings.ErrorUrl,
            CancelUrl = _settings.CancelUrl,
            ExpiryUrl = _settings.ExpiryUrl


        };
        return View("~/Plugins/Payments.WorldpayHpp/Views/Configure.cshtml", model);
        // return View("Configure", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(Models.ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS))
            return AccessDeniedView();

        _settings.EntityId = model.EntityId;
        _settings.Username = model.Username;
        _settings.Password = model.Password;
        _settings.Environment = model.Environment;
        _settings.NarrativeLine1 = model.NarrativeLine1;
        _settings.TransactionReferencePrefix = model.TransactionReferencePrefix;
        
        //_settings.UseIframe = model.UseIframe;
        _settings.SuccessUrl = model.SuccessUrl;
        _settings.PendingUrl = model.PendingUrl;
        _settings.FailureUrl = model.FailureUrl;
        _settings.ErrorUrl = model.ErrorUrl;
        _settings.CancelUrl = model.CancelUrl;
        _settings.ExpiryUrl = model.ExpiryUrl;


        await _settingService.SaveSettingAsync(_settings);
        return RedirectToAction("Configure");
    }
}



