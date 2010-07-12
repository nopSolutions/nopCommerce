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

using System;
using System.Collections.Generic;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;


namespace NopSolutions.NopCommerce.BusinessLogic.Products.Attributes
{
    /// <summary>
    /// Represents a product variant attribute mapping
    /// </summary>
    public partial class ProductVariantAttribute : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ProductVariantAttribute class
        /// </summary>
        public ProductVariantAttribute()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the product variant attribute mapping identifier
        /// </summary>
        public int ProductVariantAttributeId { get; set; }

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
        #endregion

        #region Custom Properties
        /// <summary>
        /// Gets the product variant
        /// </summary>
        public ProductVariant ProductVariant
        {
            get
            {
                return ProductManager.GetProductVariantById(this.ProductVariantId);
            }
        }

        /// <summary>
        /// Gets the product attribute
        /// </summary>
        public ProductAttribute ProductAttribute
        {
            get
            {
                return ProductAttributeManager.GetProductAttributeById(this.ProductAttributeId);
            }
        }
        
        /// <summary>
        /// Gets the product variant attribute values
        /// </summary>
        public List<ProductVariantAttributeValue> ProductVariantAttributeValues
        {
            get
            {
                return ProductAttributeManager.GetProductVariantAttributeValues(this.ProductVariantAttributeId);
            }
        }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlTypeEnum AttributeControlType
        {
            get
            {
                return (AttributeControlTypeEnum)this.AttributeControlTypeId;
            }
        }

        /// <summary>
        /// A value indicating whether this product variant attribute should have values
        /// </summary>
        public bool ShouldHaveValues
        {
            get
            {
                if (this.AttributeControlType == AttributeControlTypeEnum.TextBox ||
                    this.AttributeControlType == AttributeControlTypeEnum.MultilineTextbox ||
                    this.AttributeControlType == AttributeControlTypeEnum.Datepicker)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the product attribute
        /// </summary>
        public virtual ProductAttribute NpProductAttribute { get; set; }

        /// <summary>
        /// Gets the localized product variant attribute values
        /// </summary>
        public virtual ICollection<ProductVariantAttributeValueLocalized> NpProductVariantAttributeValueLocalized { get; set; }

        /// <summary>
        /// Gets the product variant
        /// </summary>
        public virtual ProductVariant NpProductVariant { get; set; }
        
        /// <summary>
        /// Gets the product variant attribute values
        /// </summary>
        public virtual ICollection<ProductVariantAttributeValue> NpProductVariantAttributeValues { get; set; }

        #endregion
    }

}
