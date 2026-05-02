namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the payment initiator
/// </summary>
public enum PaymentInitiatorType
{
    /// <summary>
    /// Payment is initiated with the active engagement of the customer. e.g. a customer checking out on a merchant website.
    /// </summary>
    CUSTOMER,

    /// <summary>
    /// Payment is initiated by merchant on behalf of the customer without the active engagement of customer. e.g. a merchant charging the monthly payment of a subscription to the customer.
    /// </summary>
    MERCHANT
}