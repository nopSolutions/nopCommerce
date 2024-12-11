using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.PaymentTokens;

/// <summary>
/// Represents the response to a request to get a payment token of vaulted payment source
/// </summary>
public class GetPaymentTokenResponse : PaymentToken, IApiResponse
{
}