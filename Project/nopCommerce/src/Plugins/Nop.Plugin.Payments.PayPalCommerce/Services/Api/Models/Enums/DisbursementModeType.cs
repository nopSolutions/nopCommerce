namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the disbursement mode
/// </summary>
public enum DisbursementModeType
{
    /// <summary>
    /// The funds are released to the merchant immediately.
    /// </summary>
    INSTANT,

    /// <summary>
    /// The funds are held for a finite number of days. The actual duration depends on the region and type of integration. You can release the funds through a referenced payout. Otherwise, the funds disbursed automatically after the specified duration.
    /// </summary>
    DELAYED
}