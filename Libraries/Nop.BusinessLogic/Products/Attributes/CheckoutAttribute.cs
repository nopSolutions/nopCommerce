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
    /// Represents a checkout attribute
    /// </summary>
    public partial class CheckoutAttribute : BaseEntity
    {
        #region Fields
        private List<CheckoutAttributeLocalized> _checkoutAttributeLocalized;
        #endregion

        #region Ctor
        /// <summary>
        /// Creates a new instance of the CheckoutAttribute class
        /// </summary>
        public CheckoutAttribute()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the checkout attribute identifier
        /// </summary>
        public int CheckoutAttributeId { get; set; }

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
                    if (_checkoutAttributeLocalized == null)
                        _checkoutAttributeLocalized = IoC.Resolve<ICheckoutAttributeService>().GetCheckoutAttributeLocalizedByCheckoutAttributeId(this.CheckoutAttributeId);

                    var temp1 = _checkoutAttributeLocalized.FirstOrDefault(cal => cal.LanguageId == languageId);
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

        /// <summary>
        /// Gets the localized text prompt
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized text prompt</returns>
        public string GetLocalizedTextPrompt(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_checkoutAttributeLocalized == null)
                        _checkoutAttributeLocalized = IoC.Resolve<ICheckoutAttributeService>().GetCheckoutAttributeLocalizedByCheckoutAttributeId(this.CheckoutAttributeId);

                    var temp1 = _checkoutAttributeLocalized.FirstOrDefault(cal => cal.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.TextPrompt))
                        return temp1.TextPrompt;
                }
            }

            return this.TextPrompt;
        }

        /// <summary>
        /// Gets the localized text prompt 
        /// </summary>
        public string LocalizedTextPrompt
        {
            get
            {
                return GetLocalizedTextPrompt(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        #endregion

        #region Custom Properties
        
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

        /// <summary>
        /// Gets checkout attribute values
        /// </summary>
        public List<CheckoutAttributeValue> CheckoutAttributeValues
        {
            get
            {
                return IoC.Resolve<ICheckoutAttributeService>().GetCheckoutAttributeValues(this.CheckoutAttributeId);
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the checkout attribute localized
        /// </summary>
        public virtual ICollection<CheckoutAttributeLocalized> NpCheckoutAttributeLocalized { get; set; }

        /// <summary>
        /// Gets the checkout attribute values
        /// </summary>
        public virtual ICollection<CheckoutAttributeValue> NpCheckoutAttributeValues { get; set; }

        #endregion
    }

}
