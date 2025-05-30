using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Orders;

/// <summary>
/// Represents the response to a request to authorize payment for an order
/// </summary>
public class CreateAuthorizationResponse : Order, IApiResponse
{
}