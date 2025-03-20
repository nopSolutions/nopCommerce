using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample checkout attribute
/// </summary>
public partial class SampleCheckoutAttribute
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the attribute is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the attribute control type identifier
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public AttributeControlType AttributeControlType { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether shippable products are required in order to display this attribute
    /// </summary>
    public bool ShippableProductRequired { get; set; }

    /// <summary>
    /// Gets or sets the list of checkout attribute values
    /// </summary>
    public List<SampleCheckoutAttributeValue> AttributeValues { get; set; } = new();

    #region Nested class

    /// <summary>
    /// Represents a sample checkout attribute value
    /// </summary>
    public partial class SampleCheckoutAttributeValue
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }
    }

    #endregion
}
