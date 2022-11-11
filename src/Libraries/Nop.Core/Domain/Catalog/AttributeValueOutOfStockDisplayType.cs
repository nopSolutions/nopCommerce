namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents an attribute value display type when out of stock
    /// </summary>
    public enum AttributeValueOutOfStockDisplayType
    {
        /// <summary>
        /// Attribute value is visible, but cannot be interacted
        /// </summary>
        Disable,

        /// <summary>
        /// Attribute value is display always
        /// </summary>
        AlwaysDisplay
    }
}
