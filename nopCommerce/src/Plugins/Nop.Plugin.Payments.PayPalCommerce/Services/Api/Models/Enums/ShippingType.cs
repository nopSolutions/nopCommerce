namespace Nop.Plugin.Payments.PayPalCommerce.Services.Api.Models.Enums;

/// <summary>
/// Represents the type of the shipping
/// </summary>
public enum ShippingType
{
    /// <summary>
    /// The payer intends to receive the items at a specified address.
    /// </summary>
    SHIPPING,

    /// <summary>
    /// DEPRECATED
    /// Currently, this field indicates that the payer intends to pick up the items at a specified address (ie. a store address).
    /// </summary>
    PICKUP,

    /// <summary>
    /// The payer intends to pick up the item(s) from the payee's physical store. Also termed as BOPIS, "Buy Online, Pick-up in Store". Seller protection is provided with this option.
    /// </summary>
    PICKUP_IN_STORE,

    /// <summary>
    /// The payer intends to pick up the item(s) from the payee in person. Also termed as BOPIP, "Buy Online, Pick-up in Person". Seller protection is not available, since the payer is receiving the item from the payee in person, and can validate the item prior to payment.
    /// </summary>
    PICKUP_FROM_PERSON
}