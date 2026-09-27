using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.WorldpayHpp.Constants;
using Nop.Plugin.Payments.WorldpayHpp.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;


namespace Nop.Plugin.Payments.WorldpayHpp.Controllers;


[AutoValidateAntiforgeryToken]
public class WorldpayHppPublicController : BasePublicController
{
    private readonly IOrderService _orderService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly ISettingService _settingService;
    private readonly IPermissionService _permissionService;
    private readonly ILogger _logger;
    private readonly WorldpayHppSettings _settings;

    public WorldpayHppPublicController(IOrderService orderService,
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

    // Customer redirect return - success
    [HttpGet]
    public async Task<IActionResult> Success()
    {
        try
        {

          
            return RedirectToRoute(Route.CheckoutCompleted);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("Worldpay Success handler failed", ex);
            return RedirectToRoute(Route.HomePage);
        }
    }

    // Customer redirect return - cancel
    [HttpGet]
    public IActionResult Cancel()
    {
        return View("~/Plugins/Payments.WorldpayHpp/Views/PaymentFailed.cshtml");
    }

    [HttpGet]
    public IActionResult Failure()
    {
        return View("~/Plugins/Payments.WorldpayHpp/Views/PaymentFailed.cshtml");
    }


    [HttpGet]
    public IActionResult Error()
    {
        return View("~/Plugins/Payments.WorldpayHpp/Views/PaymentFailed.cshtml");
    }



    [HttpGet]
    public IActionResult Expiry()
    {
        return View("~/Plugins/Payments.WorldpayHpp/Views/PaymentFailed.cshtml");
    }


    [HttpGet]
    public IActionResult Pending()
    {
        // Show a “processing” page
        // return RedirectToRoute("OrderDetails", new { orderId = GetOrderId(transactionReference) }); } 

        return View("~/Plugins/Payments.WorldpayHpp/Views/Pending.cshtml");
    }

  


}



