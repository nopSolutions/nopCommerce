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
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.IoC;



namespace NopSolutions.NopCommerce.BusinessLogic.Products.Attributes
{
    /// <summary>
    /// Represents a product variant attribute value
    /// </summary>
    public partial class ProductVariantAttributeValue : BaseEntity
    {
        #region Fields
        private List<ProductVariantAttributeValueLocalized> _pvavLocalized;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the ProductVariantAttributeValue class
        /// </summary>
        public ProductVariantAttributeValue()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the product variant attribute value identifier
        /// </summary>
        public int ProductVariantAttributeValueId { get; set; }

        /// <summary>
        /// Gets or sets the product variant attribute mapping identifier
        /// </summary>
        public int ProductVariantAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the product variant attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        #endregion

        #region Localizable methods/properties

        /// <summary>
        /// Gets the localized name
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized name</returns>
        public string GetLocalizedName(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_pvavLocalized == null)
                        _pvavLocalized = IoCFactory.Resolve<IProductAttributeService>().GetProductVariantAttributeValueLocalizedByProductVariantAttributeValueId(this.ProductVariantAttributeValueId);

                    var temp1 = _pvavLocalized.FirstOrDefault(cavl => cavl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.Name))
                        return temp1.Name;
                }
            }

            return this.Name;
        }

        /// <summary>
        /// Gets the localized name 
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return GetLocalizedName(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        #endregion

        #region Custom Properties
        /// <summary>
        /// Gets the product variant attribute
        /// </summary>
        public ProductVariantAttribute ProductVariantAttribute
        {
            get
            {
                return IoCFactory.Resolve<IProductAttributeService>().GetProductVariantAttributeById(this.ProductVariantAttributeId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the product variant attribute
        /// </summary>
        public virtual ProductVariantAttribute NpProductVariantAttribute { get; set; }

        /// <summary>
        /// Gets the localized product variant attribute values
        /// </summary>
        public virtual ICollection<ProductVariantAttributeValueLocalized> NpProductVariantAttributeValueLocalized { get; set; }

        #endregion
    }

}
