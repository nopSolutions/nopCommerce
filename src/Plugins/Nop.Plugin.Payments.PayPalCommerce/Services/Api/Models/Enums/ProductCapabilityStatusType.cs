namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the product capability status
/// </summary>
public enum ProductCapabilityStatusType
{
    /// <summary>
    /// The capability is enabled for the account and can be used.
    /// </summary>
    ACTIVE,

    /// <summary>
    /// The capability can no longer be used, but there are remediation steps to regain access to the corresponding functionality.
    /// </summary>
    SUSPENDED,

    /// <summary>
    /// The capability can no longer be used and there are no remediation steps available to regain the functionality.
    /// </summary>
    REVOKED,

    /// <summary>
    /// The capability is approved and could be enabled. This capability can not be used currently.
    /// </summary>
    APPROVED,

    /// <summary>
    /// Need details or documents required to enable this capability.
    /// </summary>
    NEED_DATA,

    /// <summary>
    /// The request to enable this capability is Denied by Policy Systems.
    /// </summary>
    DENY,

    /// <summary>
    /// The capability request is in review by policy system. This capability can not be used currently.
    /// </summary>
    IN_REVIEW,

    /// <summary>
    /// The capability can no longer be used and to enable this capability, please request.
    /// </summary>
    INACTIVE,

    /// <summary>
    /// The capability is in a pending state waiting for decision from policy systems.
    /// </summary>
    PENDING
}