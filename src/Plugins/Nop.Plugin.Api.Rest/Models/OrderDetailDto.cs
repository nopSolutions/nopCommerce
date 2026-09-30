using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// The full detail of an order, adding its items and addresses to the summary projection
/// </summary>
/// <remarks>
/// Declared as a class rather than a record because it extends <see cref="OrderDto"/>, which predates
/// the record style used by the newer models in this plugin
/// </remarks>
public class OrderDetailDto : OrderDto
{
    /// <summary>
    /// Gets or sets the custom order number shown to the customer
    /// </summary>
    public string? CustomOrderNumber { get; set; }

    /// <summary>
    /// Gets or sets the order GUID
    /// </summary>
    public string? OrderGuid { get; set; }

    /// <summary>
    /// Gets or sets the order subtotal including tax
    /// </summary>
    public decimal OrderSubtotalInclTax { get; set; }

    /// <summary>
    /// Gets or sets the shipping total including tax
    /// </summary>
    public decimal OrderShippingInclTax { get; set; }

    /// <summary>
    /// Gets or sets the refunded amount
    /// </summary>
    public decimal RefundedAmount { get; set; }

    /// <summary>
    /// Gets or sets the date and time the order was paid (UTC)
    /// </summary>
    public DateTime? PaidDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the shipping method
    /// </summary>
    public string? ShippingMethod { get; set; }

    /// <summary>
    /// Gets or sets the payment method system name
    /// </summary>
    public string? PaymentMethodSystemName { get; set; }

    /// <summary>
    /// Gets or sets the checkout attribute description
    /// </summary>
    public string? CheckoutAttributeDescription { get; set; }

    /// <summary>
    /// Gets or sets the items of the order
    /// </summary>
    public List<OrderItemDto> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the billing address, which also carries the billing email and phone
    /// </summary>
    public AddressDto? BillingAddress { get; set; }

    /// <summary>
    /// Gets or sets the shipping address, falling back to the billing address when the order has none
    /// </summary>
    public AddressDto? ShippingAddress { get; set; }
}

/// <summary>
/// A single line item of an order
/// </summary>
public record OrderItemDto
{
    /// <summary>
    /// Gets or sets the order item identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the ordered quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price including tax
    /// </summary>
    public decimal UnitPriceInclTax { get; set; }

    /// <summary>
    /// Gets or sets the line total including tax
    /// </summary>
    public decimal PriceInclTax { get; set; }

    /// <summary>
    /// Gets or sets the discount amount including tax
    /// </summary>
    public decimal DiscountAmountInclTax { get; set; }

    /// <summary>
    /// Gets or sets the product cost recorded at the time of the order
    /// </summary>
    public decimal OriginalProductCost { get; set; }

    /// <summary>
    /// Gets or sets the product name, resolved from the product record
    /// </summary>
    /// <remarks>
    /// The order item does not store the name, so a deleted or renamed product can leave this empty
    /// </remarks>
    public string? ProductName { get; set; }

    /// <summary>
    /// Gets or sets the SKU, resolved from the product record
    /// </summary>
    public string? ProductSku { get; set; }

    /// <summary>
    /// Gets or sets the human readable attribute description
    /// </summary>
    public string? AttributeDescription { get; set; }

    /// <summary>
    /// Gets or sets the stored attributes XML
    /// </summary>
    public string? AttributesXml { get; set; }
}
