using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.DropShipping.AliExpress.Services;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using System.Text.Json;
using ClosedXML.Excel;

namespace Nop.Plugin.DropShipping.AliExpress.Controllers;

/// <summary>
/// Controller for handling AliExpress webhooks
/// </summary>
[Route("Plugins/AliExpressWebhook")]
[ApiController]
public class AliExpressWebhookController : BaseController
{
    private readonly IAliExpressOrderTrackingService _trackingService;
    private readonly ILogger _logger;

    public AliExpressWebhookController(
        IAliExpressOrderTrackingService trackingService,
        ILogger logger)
    {
        _trackingService = trackingService;
        _logger = logger;
    }

    /// <summary>
    /// Receives order status updates from AliExpress
    /// </summary>
    [HttpPost("OrderStatus")]
    public async Task<IActionResult> OrderStatus()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            await _logger.InformationAsync($"Received AliExpress webhook: {body}");

            // Parse the webhook payload
            var payload = JsonSerializer.Deserialize<JsonElement>(body);

            // Extract relevant information based on AliExpress webhook structure
            // Note: Actual structure depends on AliExpress API documentation
            if (payload.TryGetProperty("order_id", out var orderIdElement) &&
                payload.TryGetProperty("status", out var statusElement))
            {
                var orderId = orderIdElement.GetInt64();
                var status = statusElement.GetString();

                if (string.IsNullOrEmpty(status))
                {
                    return BadRequest("Invalid status");
                }

                // Update tracking information
                await _trackingService.UpdateTrackingAsync(orderId, status);

                // If status indicates delivery, process accordingly
                if (status.Contains("delivered", StringComparison.OrdinalIgnoreCase))
                {
                    await _trackingService.ProcessDeliveryNotificationAsync(orderId);
                }

                return Ok(new { success = true, message = "Webhook processed" });
            }

            return BadRequest("Invalid webhook payload");
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error processing AliExpress webhook: {ex.Message}", ex);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Receives tracking updates from AliExpress
    /// </summary>
    [HttpPost("Tracking")]
    public async Task<IActionResult> Tracking()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            await _logger.InformationAsync($"Received AliExpress tracking webhook: {body}");

            var payload = JsonSerializer.Deserialize<JsonElement>(body);

            if (payload.TryGetProperty("order_id", out var orderIdElement) &&
                payload.TryGetProperty("tracking_number", out var trackingElement))
            {
                var orderId = orderIdElement.GetInt64();
                var trackingNumber = trackingElement.GetString();

                if (!string.IsNullOrEmpty(trackingNumber))
                {
                    await _trackingService.UpdateTrackingAsync(orderId, "Shipped", trackingNumber);
                }

                return Ok(new { success = true, message = "Tracking updated" });
            }

            return BadRequest("Invalid tracking payload");
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error processing AliExpress tracking webhook: {ex.Message}", ex);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("Health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
