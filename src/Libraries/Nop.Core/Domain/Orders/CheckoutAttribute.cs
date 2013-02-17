using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a checkout attribute
    /// </summary>
    public partial class CheckoutAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<CheckoutAttributeValue> _checkoutAttributeValues;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text prompt
        /// </summary>
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public bool IsRequired { get; set; }

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
            get
            {
                return (AttributeControlType)this.AttributeControlTypeId;
            }
            set
            {
                this.AttributeControlTypeId = (int)value;
            }
        }
        /// <summary>
        /// Gets the checkout attribute values
        /// </summary>
        public virtual ICollection<CheckoutAttributeValue> CheckoutAttributeValues
        {
            get { return _checkoutAttributeValues ?? (_checkoutAttributeValues = new List<CheckoutAttributeValue>()); }
            protected set { _checkoutAttributeValues = value; }
        }
    }

}
