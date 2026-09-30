using System.Collections.Generic;

namespace Nop.Plugin.Api.Rest.Models;

/// <summary>
/// An attribute mapped to a product, together with the values that can be selected for it
/// </summary>
public record ProductAttributeDto
{
    /// <summary>
    /// Gets or sets the product attribute identifier
    /// </summary>
    public int ProductAttributeId { get; set; }

    /// <summary>
    /// Gets or sets the attribute mapping identifier, which is what a combination refers to
    /// </summary>
    public int MappingId { get; set; }

    /// <summary>
    /// Gets or sets the attribute name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the text prompt shown instead of the attribute name
    /// </summary>
    public string? TextPrompt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the attribute is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the control type identifier, which decides how the value is entered
    /// </summary>
    public int AttributeControlTypeId { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the values that can be selected for the attribute
    /// </summary>
    public List<ProductAttributeValueDto> Values { get; set; } = new();
}

/// <summary>
/// A selectable value of a product attribute
/// </summary>
public record ProductAttributeValueDto
{
    /// <summary>
    /// Gets or sets the attribute value identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the value name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the stored value
    /// </summary>
    public int AttributeValueTypeId { get; set; }

    /// <summary>
    /// Gets or sets the associated product identifier, used by the grouped product control
    /// </summary>
    public int AssociatedProductId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value is pre selected
    /// </summary>
    public bool IsPreSelected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the customer enters the quantity
    /// </summary>
    public bool CustomerEntersQty { get; set; }

    /// <summary>
    /// Gets or sets the quantity, when the customer does not enter it
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price adjustment applied when the value is selected
    /// </summary>
    public decimal PriceAdjustment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the price adjustment is a percentage
    /// </summary>
    public bool PriceAdjustmentUsePercentage { get; set; }

    /// <summary>
    /// Gets or sets the weight adjustment applied when the value is selected
    /// </summary>
    public decimal WeightAdjustment { get; set; }

    /// <summary>
    /// Gets or sets the RGB value used to render the swatch
    /// </summary>
    public string? ColorSquaresRgb { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}
