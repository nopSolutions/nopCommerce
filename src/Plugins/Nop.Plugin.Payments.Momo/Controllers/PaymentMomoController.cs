using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.Momo.Models;
using Nop.Plugin.Payments.Momo.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.Momo.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class PaymentMomoController : BasePaymentController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IMomoPaymentService _momoPaymentService;

    #endregion

    #region Ctor

    public PaymentMomoController(
        ILocalizationService localizationService,
        INotificationService notificationService,
        IMomoPaymentService momoPaymentService)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _momoPaymentService = momoPaymentService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> ConfigureAsync()
    {
        var model = await _momoPaymentService.GetConfigurationModelAsync();
        return View("~/Plugins/Payments.Momo/Views/Configure.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> ConfigureAsync(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await ConfigureAsync();

        await _momoPaymentService.SaveConfigurationAsync(model);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await ConfigureAsync();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> CreateApiUserAsync(ConfigurationModel model)
    {
        var (success, message) = await _momoPaymentService.CreateApiUserAsync(model.ApiUser);

        if (success)
            _notificationService.SuccessNotification(message);
        else
            _notificationService.ErrorNotification(message);

        return RedirectToAction("Configure");
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_PAYMENT_METHODS)]
    public async Task<IActionResult> GenerateApiKeyAsync(ConfigurationModel model)
    {
        var (success, message) = await _momoPaymentService.GenerateApiKeyAsync();

        if (success)
            _notificationService.SuccessNotification(message);
        else
            _notificationService.ErrorNotification(message);

        return RedirectToAction(nameof(ConfigureAsync));
    }

    #endregion

    #region Public Methods

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CallbackAsync([FromBody] MomoCallbackModel model)
    {
        var (success, message) = await _momoPaymentService.ProcessCallbackAsync(model);

        if (success)
            return Ok();

        return StatusCode(500, message);
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> InitiatePaymentAsync([FromBody] MomoRequest momoRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

        var (success, referenceId, message) = await _momoPaymentService.InitiatePaymentAsync(momoRequest.PhoneNumber);

        return Json(new
        {
            success,
            referenceId,
            message
        });
    }

    [HttpGet]
    public async Task<IActionResult> CheckStatusAsync(string referenceId)
    {
        if (string.IsNullOrEmpty(referenceId))
            return Json(new { success = false, message = "Reference ID is required" });

        var (success, status, message, redirectUrl) = await _momoPaymentService.CheckPaymentStatusAsync(referenceId);

        return Json(new
        {
            success,
            status,
            message,
            redirectUrl
        });
    }

    #endregion

    #region Classes


    public class MomoRequest
    {
        [Required, StringLength(15, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 15 characters long")]
        public string PhoneNumber { get; set; }
    }

    #endregion
}