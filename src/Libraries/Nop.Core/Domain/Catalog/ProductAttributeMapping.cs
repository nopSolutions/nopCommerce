using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product attribute mapping
    /// </summary>
    public partial class ProductAttributeMapping : BaseEntity, ILocalizedEntity
    {
        private ICollection<ProductAttributeValue> _productAttributeValues;

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product attribute identifier
        /// </summary>
        public int ProductAttributeId { get; set; }

        /// <summary>
        /// Gets or sets a value a text prompt
        /// </summary>
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
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
        /// Leave empty (or null) to enable this attribute.
        /// Conditional attributes that only appear if a previous attribute is selected, such as having an option 
        /// for personalizing clothing with a name and only providing the text input box if the "Personalize" radio button is checked.
        /// </summary>
        public string ConditionAttributeXml { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get
            {
                return (AttributeControlType)AttributeControlTypeId;
            }
            set
            {
                AttributeControlTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the product attribute
        /// </summary>
        public virtual ProductAttribute ProductAttribute { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets the product attribute values
        /// </summary>
        public virtual ICollection<ProductAttributeValue> ProductAttributeValues
        {
            get { return _productAttributeValues ?? (_productAttributeValues = new List<ProductAttributeValue>()); }
            protected set { _productAttributeValues = value; }
        }

    }

}
