namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the tenure
/// </summary>
public enum TenureType
{
    /// <summary>
    /// A regular billing cycle to identify recurring charges for the billing agreement.
    /// </summary>
    REGULAR,

    /// <summary>
    /// A trial billing cycle to identify free or discounted charge for the billing agreement. Free trails will not have a price object in pricing scheme where as a discounted trial would have a discounted price compared to regular billing cycle.
    /// </summary>
    TRIAL
}