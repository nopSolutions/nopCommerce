namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the feature
/// </summary>
public enum FeatureType
{
    /// <summary>
    /// Payout feature.
    /// </summary>
    PAYOUTS,

    /// <summary>
    /// Payment feature.
    /// </summary>
    PAYMENT,

    /// <summary>
    /// Refund feature.
    /// </summary>
    REFUND,

    /// <summary>
    /// Future Payment feature.
    /// </summary>
    FUTURE_PAYMENT,

    /// <summary>
    /// Direct Payment feature.
    /// </summary>
    DIRECT_PAYMENT,

    /// <summary>
    /// Partner fee feature.
    /// </summary>
    PARTNER_FEE,

    /// <summary>
    /// Delay funds disbursement feature.
    /// </summary>
    DELAY_FUNDS_DISBURSEMENT,

    /// <summary>
    /// Read seller dispute feature.
    /// </summary>
    READ_SELLER_DISPUTE,

    /// <summary>
    /// update seller dispute feature.
    /// </summary>
    UPDATE_SELLER_DISPUTE,

    /// <summary>
    /// Advanced transaction search feature.
    /// </summary>
    ADVANCED_TRANSACTIONS_SEARCH,

    /// <summary>
    /// Sweep funds external sink feature.
    /// </summary>
    SWEEP_FUNDS_EXTERNAL_SINK,

    /// <summary>
    /// Access merchant information feature.
    /// </summary>
    ACCESS_MERCHANT_INFORMATION,

    /// <summary>
    /// Tracking Shipment readwrite feature.
    /// </summary>
    TRACKING_SHIPMENT_READWRITE,

    /// <summary>
    /// Invoice readwrite feature.
    /// </summary>
    INVOICE_READ_WRITE,

    /// <summary>
    /// Read the buyer disputes.
    /// </summary>
    DISPUTE_READ_BUYER,

    /// <summary>
    /// Update the buyer disputes.
    /// </summary>
    UPDATE_CUSTOMER_DISPUTES,

    /// <summary>
    /// Manage the payment methods that my customers save.
    /// </summary>
    VAULT,

    /// <summary>
    /// Obtain prior approval for future payments(billing agreements).
    /// </summary>
    BILLING_AGREEMENT
}