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

namespace Nop.Core.Domain
{
    /// <summary>
    /// Represents a language
    /// </summary>
    public partial class Language : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the language culture
        /// </summary>
        public string LanguageCulture { get; set; }

        /// <summary>
        /// Gets or sets the flag image file name
        /// </summary>
        public string FlagImageFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the language is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets locale string resources
        /// </summary>
        public virtual ICollection<LocaleStringResource> LocaleStringResources { get; set; }

        /// <summary>
        /// Gets or sets localized categories
        /// </summary>
        public virtual ICollection<LocalizedCategory> LocalizedCategories { get; set; }

        /// <summary>
        /// Gets or sets localized manufacturers
        /// </summary>
        public virtual ICollection<LocalizedManufacturer> LocalizedManufacturers { get; set; }

        /// <summary>
        /// Gets or sets localized products
        /// </summary>
        public virtual ICollection<LocalizedProduct> LocalizedProducts { get; set; }

        /// <summary>
        /// Gets or sets localized product variants
        /// </summary>
        public virtual ICollection<LocalizedProductVariant> LocalizedProductVariants { get; set; }
    }
}
