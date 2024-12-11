using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AmazonPay.Models;
using Nop.Plugin.Payments.AmazonPay.Services;

namespace Nop.Plugin.Payments.AmazonPay.Controllers;

public class AmazonPayOnboardingController : Controller
{
    #region Fields

    private readonly AmazonPayOnboardingService _amazonPayOnboardingService;

    #endregion

    #region Ctor

    public AmazonPayOnboardingController(AmazonPayOnboardingService amazonPayOnboardingService)
    {
        _amazonPayOnboardingService = amazonPayOnboardingService;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> KeyShare(KeyExchangeModel model)
    {
        if (Uri.TryCreate(Request.Headers.Origin, UriKind.Absolute, out var uri) && AmazonPayDefaults.Onboarding.OriginUrls.Contains(uri.Host))
            Response.Headers.TryAdd("Access-Control-Allow-Origin", $"{Uri.UriSchemeHttps}{Uri.SchemeDelimiter}{uri.Host}");
        Response.Headers.TryAdd("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        Response.Headers.TryAdd("Access-Control-Allow-Headers", "Content-Type, X-CSRF-Token");

        var error = await _amazonPayOnboardingService.AutomaticKeyExchangeAsync(model.Payload);

        if (string.IsNullOrEmpty(error))
            return Json(new { result = "success" });

        return new JsonResult(new { result = "error", message = error }) { StatusCode = 400 };

    }

    #endregion
}