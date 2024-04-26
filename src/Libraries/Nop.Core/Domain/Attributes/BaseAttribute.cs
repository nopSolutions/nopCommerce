using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Attributes;

/// <summary>
/// Represents the base class for attributes
/// </summary>
public abstract partial class BaseAttribute : BaseEntity, ILocalizedEntity
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
    public int AttributeControlTypeId { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets the attribute control type
    /// </summary>
    public AttributeControlType AttributeControlType
    {
        get => (AttributeControlType)AttributeControlTypeId;
        set => AttributeControlTypeId = (int)value;
    }

    /// <summary>
    /// A value indicating whether this attribute should have values
    /// </summary>
    public bool ShouldHaveValues
    {
        get
        {
            if (AttributeControlType == AttributeControlType.TextBox ||
                AttributeControlType == AttributeControlType.MultilineTextbox ||
                AttributeControlType == AttributeControlType.Datepicker ||
                AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support values
            return true;
        }
    }

    /// <summary>
    /// A value indicating whether this attribute can be used as condition for some other attribute
    /// </summary>
    public bool CanBeUsedAsCondition
    {
        get
        {
            if (AttributeControlType == AttributeControlType.ReadonlyCheckboxes ||
                AttributeControlType == AttributeControlType.TextBox ||
                AttributeControlType == AttributeControlType.MultilineTextbox ||
                AttributeControlType == AttributeControlType.Datepicker ||
                AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support it
            return true;
        }
    }
}