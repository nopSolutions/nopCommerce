namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the category
/// </summary>
public enum CategoryType
{
    /// <summary>
    /// Goods that are stored, delivered, and used in their electronic format. This value is not currently supported for API callers that leverage the [PayPal for Commerce Platform](https://www.paypal.com/us/webapps/mpp/commerce-platform) product.
    /// </summary>
    DIGITAL_GOODS,

    /// <summary>
    /// A tangible item that can be shipped with proof of delivery.
    /// </summary>
    PHYSICAL_GOODS,

    /// <summary>
    /// A contribution or gift for which no good or service is exchanged, usually to a not for profit organization.
    /// </summary>
    DONATION
}