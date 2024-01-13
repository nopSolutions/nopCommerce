namespace Nop.Core.Domain.Catalog;

/// <summary>
/// Represents an attribute value type
/// </summary>
public enum AttributeValueType
{
    /// <summary>
    /// Simple attribute value
    /// </summary>
    Simple = 0,

    /// <summary>
    /// Associated to a product (used when configuring bundled products)
    /// </summary>
    AssociatedToProduct = 10,
}