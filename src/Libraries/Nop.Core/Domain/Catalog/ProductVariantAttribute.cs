//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product variant attribute mapping
    /// </summary>
    public partial class ProductVariantAttribute : BaseEntity, ILocalizedEntity
    {
        public ProductVariantAttribute() 
        {
            this.ProductVariantAttributeValues = new List<ProductVariantAttributeValue>();
        }

        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public int ProductVariantId { get; set; }

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

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlTypeEnum AttributeControlType
        {
            get
            {
                return (AttributeControlTypeEnum)this.AttributeControlTypeId;
            }
            set
            {
                this.AttributeControlTypeId = (int)value; 
            }
        }

        /// <summary>
        /// Gets the product attribute
        /// </summary>
        public virtual ProductAttribute ProductAttribute { get; set; }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual ProductVariant ProductVariant { get; set; }
        
        /// <summary>
        /// Gets the product variant attribute values
        /// </summary>
        public virtual ICollection<ProductVariantAttributeValue> ProductVariantAttributeValues { get; set; }

    }

}
