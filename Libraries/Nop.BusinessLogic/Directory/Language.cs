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
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Directory
{
    /// <summary>
    /// Represents a language
    /// </summary>
    public partial class Language : BaseEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

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

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets an icon URL
        /// </summary>
        public string IconUrl
        {
            get
            {
                string url = string.Empty;
                if (!String.IsNullOrEmpty(this.FlagImageFileName))
                {
                    url = string.Format("{0}images/flags/{1}", CommonHelper.GetStoreLocation(), this.FlagImageFileName);
                }
                return url;
            }
        }
        #endregion
    }
}
