using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.AmazonPay.Services;

namespace Nop.Plugin.Payments.AmazonPay.Controllers;

public class AmazonPayIpnController(AmazonPayPaymentService amazonPayPaymentService) : Controller
{
    #region Methods

    [HttpPost]
    public async Task<IActionResult> IPNHandler()
    {
        await amazonPayPaymentService.ProcessIPNRequestAsync();

        return Ok();
    }

    #endregion
}