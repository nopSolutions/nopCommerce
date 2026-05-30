namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the usage pattern
/// </summary>
public enum UsagePatternType
{
    /// <summary>
    /// On-demand instant payments – non-recurring, pre-paid, variable amount, variable frequency.
    /// </summary>
    IMMEDIATE,

    /// <summary>
    /// Pay after use, non-recurring post-paid, variable amount, irregular frequency.
    /// </summary>
    DEFERRED,

    /// <summary>
    /// Subscription plan where the amount due and the billing frequency are fixed. There is no defined duration for the payment due before the goods or services are delivered.
    /// </summary>
    SUBSCRIPTION_PREPAID,

    /// <summary>
    /// Subscription plan where the amount due and the billing frequency are fixed. There is no defined duration for the payment due after the goods or services are delivered.
    /// </summary>
    SUBSCRIPTION_POSTPAID,

    /// <summary>
    /// Pay a fixed or variable amount upfront on a fixed date before the goods or services are delivered.
    /// </summary>
    RECURRING_PREPAID,

    /// <summary>
    /// Pay on a fixed date based on usage or consumption after the goods or services are delivered.
    /// </summary>
    RECURRING_POSTPAID,

    /// <summary>
    /// Unscheduled card-on-file plan where the merchant can bill the payer upfront based on an agreed logic, but the amount due and frequency can vary. This includes automatic reload plans.
    /// </summary>
    UNSCHEDULED_PREPAID,

    /// <summary>
    /// Merchant-managed installment plan when the amount to be paid and the billing frequency is fixed, but there is a defined number of payments with the payment due after the goods or services are delivered.
    /// </summary>
    UNSCHEDULED_POSTPAID,

    /// <summary>
    /// Merchant-managed installment plan when the amount to be paid and the billing frequency are fixed, but there is a defined number of payments with the payment due before the gods or services are delivered.
    /// </summary>
    INSTALLMENT_PREPAID,

    /// <summary>
    /// Merchant-managed installment plan when the amount to be paid and the billing frequency are fixed, but there is a defined number of payments with the payment due after the goods or services are delivered.
    /// </summary>
    INSTALLMENT_POSTPAID,

    /// <summary>
    /// Charge payer when the set amount is reached or monthly billing cycle, whichever comes first, before the goods/service is delivered.
    /// </summary>
    THRESHOLD_PREPAID,

    /// <summary>
    /// Charge payer when the set amount is reached or monthly billing cycle, whichever comes first, after the goods/service is delivered.
    /// </summary>
    THRESHOLD_POSTPAID
}