namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the payment
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// One Time payment such as online purchase or donation. (e.g. Checkout with one-click).
    /// </summary>
    ONE_TIME,

    /// <summary>
    /// Payment which is part of a series of payments with fixed or variable amounts, following a fixed time interval. (e.g. Subscription payments).
    /// </summary>
    RECURRING,

    /// <summary>
    /// Payment which is part of a series of payments that occur on a non-fixed schedule and/or have variable amounts. (e.g. Account Topup payments).
    /// </summary>
    UNSCHEDULED
}