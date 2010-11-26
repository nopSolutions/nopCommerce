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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Attributes
{
    /// <summary>
    /// Represents a checkout attribute value
    /// </summary>
    public partial class CheckoutAttributeValue : BaseEntity
    {
        #region Fields
        private List<CheckoutAttributeValueLocalized> _cavLocalized;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the checkout attribute value identifier
        /// </summary>
        public int CheckoutAttributeValueId { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute mapping identifier
        /// </summary>
        public int CheckoutAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the checkout attribute name
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
                    if (_cavLocalized == null)
                        _cavLocalized = IoC.Resolve<ICheckoutAttributeService>().GetCheckoutAttributeValueLocalizedByCheckoutAttributeValueId(this.CheckoutAttributeValueId);

                    var temp1 = _cavLocalized.FirstOrDefault(cavl => cavl.LanguageId == languageId);
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
        /// Gets the checkout attribute
        /// </summary>
        public CheckoutAttribute CheckoutAttribute
        {
            get
            {
                return IoC.Resolve<ICheckoutAttributeService>().GetCheckoutAttributeById(this.CheckoutAttributeId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the checkout attribute
        /// </summary>
        public virtual CheckoutAttribute NpCheckoutAttribute { get; set; }

        /// <summary>
        /// Gets the localized checkout attribute values
        /// </summary>
        public virtual ICollection<CheckoutAttributeValueLocalized> NpCheckoutAttributeValueLocalized { get; set; }

        #endregion
    }

}
