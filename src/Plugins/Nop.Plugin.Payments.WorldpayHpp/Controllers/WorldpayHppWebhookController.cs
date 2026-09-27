using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Payments.WorldpayHpp.Handlers;
using Nop.Plugin.Payments.WorldpayHpp.Models;
using Nop.Plugin.Payments.WorldpayHpp.Models.Webhook;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;

namespace Nop.Plugin.Payments.WorldpayHpp.Controllers;

public class WorldpayHppWebhookController : Controller
{
    
    private readonly ILogger _logger;
    private readonly IWorldpayWebhookHandler _worldpayWebhookHandler;


    public WorldpayHppWebhookController(
                                 ILogger logger,
                                 WorldpayHppSettings settings,
                                 IWorldpayWebhookHandler worldpayWebhookHandler
        )
    {
        
        _logger = logger;
        _worldpayWebhookHandler = worldpayWebhookHandler;
    }

    [HttpPost]
    [IgnoreAntiforgeryToken] 
    public async Task<IActionResult> WebhookHandler() {
        
        try
        {
          
           var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var worldpayWebhookEnvelope = await Request.ReadFromJsonAsync<WorldpayWebhookEnvelope>(jsonOptions);

            await _logger.InformationAsync($"Received Worldpay webhook: {worldpayWebhookEnvelope.EventId}");
            await _worldpayWebhookHandler.HandleAsync(worldpayWebhookEnvelope);
            return Ok();
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error handling Worldpay webhook", ex);
        }

        return StatusCode(StatusCodes.Status500InternalServerError);

    }
}



