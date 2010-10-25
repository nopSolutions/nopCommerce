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
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Products.Specs
{
    /// <summary>
    /// Represents a specification attribute
    /// </summary>
    public partial class SpecificationAttribute : BaseEntity
    {
        #region Fields
        private List<SpecificationAttributeLocalized> _saLocalized;
        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new instance of the SpecificationAttribute class
        /// </summary>
        public SpecificationAttribute()
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        public int SpecificationAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

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
                    if (_saLocalized == null)
                        _saLocalized = IoCFactory.Resolve<ISpecificationAttributeManager>().GetSpecificationAttributeLocalizedBySpecificationAttributeId(this.SpecificationAttributeId);

                    var temp1 = _saLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
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

        #region Navigation Properties

        /// <summary>
        /// Gets the localized specification attribute
        /// </summary>
        public virtual ICollection<SpecificationAttributeLocalized> NpSpecificationAttributeLocalized { get; set; }

        /// <summary>
        /// Gets the specification attribute options
        /// </summary>
        public virtual ICollection<SpecificationAttributeOption> NpSpecificationAttributeOptions { get; set; }

        #endregion
    }
}
