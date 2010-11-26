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
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.Content.NewsManagement
{
    /// <summary>
    /// Represents a news
    /// </summary>
    public partial class News : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the News class
        /// </summary>
        public News()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the news identifier
        /// </summary>
        public int NewsId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the news title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the short text
        /// </summary>
        public string Short { get; set; }

        /// <summary>
        /// Gets or sets the full text
        /// </summary>
        public string Full { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity allows comments
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion 

        #region Custom Properties
        /// <summary>
        /// Gets the language
        /// </summary>
        public Language Language
        {
            get
            {
                return IoC.Resolve<ILanguageService>().GetLanguageById(this.LanguageId);
            }
        }

        /// <summary>
        /// Gets the news comments
        /// </summary>
        public List<NewsComment> NewsComments
        {
            get
            {
                return IoC.Resolve<INewsService>().GetNewsCommentsByNewsId(this.NewsId);
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the news comments
        /// </summary>
        public virtual ICollection<NewsComment> NpNewsComments { get; set; }

        /// <summary>
        /// Gets the language
        /// </summary>
        public virtual Language NpLanguage { get; set; }
        
        #endregion
    }

}
