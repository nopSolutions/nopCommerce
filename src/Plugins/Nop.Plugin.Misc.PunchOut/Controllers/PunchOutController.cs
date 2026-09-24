using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Http;
using Nop.Plugin.Misc.PunchOut.Domain.CXML;
using Nop.Plugin.Misc.PunchOut.Services;
using Nop.Services.Authentication;
using Nop.Services.Localization;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.PunchOut.Controllers;

[Route("punchout")]
public class PunchOutController : Controller
{
    #region Fields

    protected readonly IAuthenticationService _authenticationService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly PunchOutService _punchOutService;
    protected readonly PunchOutSettings _punchOutSettings;

    #endregion

    #region Ctor

    public PunchOutController(IAuthenticationService authenticationService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        PunchOutService punchOutService,
        PunchOutSettings punchOutSettings)
    {
        _authenticationService = authenticationService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _punchOutService = punchOutService;
        _punchOutSettings = punchOutSettings;
    }

    #endregion

    #region Methods

    [HttpPost("setup")]
    public async Task<IActionResult> Setup()
    {
        if (!_punchOutSettings.IsActive)
        {
            var response = PunchOutXmlBuilder.BuildErrorResponse(new PunchOutErrorResponse
            {
                StatusCode = "503",
                StatusText = "Service Unavailable",
                ErrorMessage = await _localizationService.GetResourceAsync("Plugins.Misc.PunchOut.ServiceUnavailable")
            });
            return Content(response, "text/xml", Encoding.UTF8);
        }

        string rawXml;
        using (var reader = new StreamReader(Request.Body))
            rawXml = await reader.ReadToEndAsync();

        // parse PunchOutSetupRequest and create session
        var responseXml = await _punchOutService.HandleSetupRequestAsync(rawXml, HttpContext);

        return Content(responseXml, "text/xml", Encoding.UTF8);
    }

    [HttpGet("start")]
    public async Task<IActionResult> Start([FromQuery] string sessionId)
    {
        if (!_punchOutSettings.IsActive)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Misc.PunchOut.ServiceUnavailable"));
            return new RedirectToRouteResult(NopRouteNames.General.HOMEPAGE, null);
        }

        var result = await _punchOutService.StartSessionAsync(sessionId);
        if (result.Customer != null)
        {
            await _authenticationService.SignInAsync(result.Customer, false);
            return new RedirectToRouteResult(NopRouteNames.General.HOMEPAGE, null);
        }

        return StatusCode(StatusCodes.Status401Unauthorized);
    }

    [HttpGet("return")]
    public async Task<IActionResult> ReturnToProcurement()
    {
        if (!_punchOutSettings.IsActive)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Misc.PunchOut.ServiceUnavailable"));
            return new RedirectToRouteResult(NopRouteNames.General.HOMEPAGE, null);
        }

        var result = await _punchOutService.BuildReturnResponseAsync();

        await _punchOutService.ClearPunchoutSessionDataAsync();

        return Content(result, "text/html", Encoding.UTF8);
    }

    #endregion
}
