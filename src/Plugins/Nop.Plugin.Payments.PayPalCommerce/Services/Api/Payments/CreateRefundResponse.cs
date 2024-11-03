using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Payments;

/// <summary>
/// Represents the response to a request to refund a captured payment
/// </summary>
public class CreateRefundResponse : Refund, IApiResponse
{
}