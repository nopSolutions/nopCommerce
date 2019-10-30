using LinqToDB.Mapping;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product attribute mapping
    /// </summary>
    [Table(NopMappingDefaults.ProductProductAttributeTable)]
    public partial class ProductAttributeMapping : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [Column]
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product attribute identifier
        /// </summary>
        [Column]
        public int ProductAttributeId { get; set; }

        /// <summary>
        /// Gets or sets a value a text prompt
        /// </summary>
        [Column]
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        [Column]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        [Column]
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [Column]
        public int DisplayOrder { get; set; }

        //validation fields

        /// <summary>
        /// Gets or sets the validation rule for minimum length (for textbox and multiline textbox)
        /// </summary>
        [Column]
        public int? ValidationMinLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for maximum length (for textbox and multiline textbox)
        /// </summary>
        [Column]
        public int? ValidationMaxLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file allowed extensions (for file upload)
        /// </summary>
        [Column]
        public string ValidationFileAllowedExtensions { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file maximum size in kilobytes (for file upload)
        /// </summary>
        [Column]
        public int? ValidationFileMaximumSize { get; set; }

        /// <summary>
        /// Gets or sets the default value (for textbox and multiline textbox)
        /// </summary>
        [Column]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a condition (depending on other attribute) when this attribute should be enabled (visible).
        /// Leave empty (or null) to enable this attribute.
        /// Conditional attributes that only appear if a previous attribute is selected, such as having an option 
        /// for personalizing clothing with a name and only providing the text input box if the "Personalize" radio button is checked.
        /// </summary>
        [Column]
        public string ConditionAttributeXml { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        [NotColumn]
        public AttributeControlType AttributeControlType
        {
            get => (AttributeControlType)AttributeControlTypeId;
            set => AttributeControlTypeId = (int)value;
        }
    }
}