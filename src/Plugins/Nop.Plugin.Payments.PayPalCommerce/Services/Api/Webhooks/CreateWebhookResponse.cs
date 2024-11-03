using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Webhooks;

/// <summary>
/// Represents the response to a request to subscribe a webhook listener to events
/// </summary>
public class CreateWebhookResponse : Webhook, IApiResponse
{
}