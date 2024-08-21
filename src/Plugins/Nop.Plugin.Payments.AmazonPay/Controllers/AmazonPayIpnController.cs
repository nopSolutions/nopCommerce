using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AmazonPay.Services;

namespace Nop.Plugin.Payments.AmazonPay.Controllers;

public class AmazonPayIpnController : Controller
{
    #region Fields

    private readonly AmazonPayPaymentService _amazonPayPaymentService;

    #endregion

    #region Ctor

    public AmazonPayIpnController(AmazonPayPaymentService amazonPayPaymentService)
    {
        _amazonPayPaymentService = amazonPayPaymentService;
    }

    #endregion

    #region Methods

    [HttpPost]
    public async Task<IActionResult> IPNHandler()
    {
        await _amazonPayPaymentService.ProcessIPNRequestAsync();

        return Ok();
    }

    #endregion
}