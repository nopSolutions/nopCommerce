using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Payments;

/// <summary>
/// Represents the response to a request to capture an authorized payment
/// </summary>
public class CreateCaptureResponse : Capture, IApiResponse
{
}