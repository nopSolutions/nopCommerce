using Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models;

namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.PaymentTokens;

/// <summary>
/// Represents the response to a request to create a Setup Token from the given payment source and adds it to the Vault of the associated customer
/// </summary>
public class CreateSetupTokenResponse : PaymentToken, IApiResponse
{
}