using Nop.Core.Domain.Attributes;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Represents a checkout attribute
/// </summary>
public partial class CheckoutAttribute : BaseAttribute, IStoreMappingSupported
{
    /// <summary>
    /// Gets or sets the text prompt
    /// </summary>
    public string TextPrompt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether shippable products are required in order to display this attribute
    /// </summary>
    public bool ShippableProductRequired { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the attribute is marked as tax exempt
    /// </summary>
    public bool IsTaxExempt { get; set; }

    /// <summary>
    /// Gets or sets the tax category identifier
    /// </summary>
    public int TaxCategoryId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public bool LimitedToStores { get; set; }

    //validation fields

    /// <summary>
    /// Gets or sets the validation rule for minimum length (for textbox and multiline textbox)
    /// </summary>
    public int? ValidationMinLength { get; set; }

    /// <summary>
    /// Gets or sets the validation rule for maximum length (for textbox and multiline textbox)
    /// </summary>
    public int? ValidationMaxLength { get; set; }

    /// <summary>
    /// Gets or sets the validation rule for file allowed extensions (for file upload)
    /// </summary>
    public string ValidationFileAllowedExtensions { get; set; }

    /// <summary>
    /// Gets or sets the validation rule for file maximum size in kilobytes (for file upload)
    /// </summary>
    public int? ValidationFileMaximumSize { get; set; }

    /// <summary>
    /// Gets or sets the default value (for textbox and multiline textbox)
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets a condition (depending on other attribute) when this attribute should be enabled (visible).
    /// </summary>
    public string ConditionAttributeXml { get; set; }
}