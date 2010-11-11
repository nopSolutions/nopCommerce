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
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Content.Topics
{
    /// <summary>
    /// Represents a localized topic
    /// </summary>
    public partial class LocalizedTopic : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the LocalizedTopic class
        /// </summary>
        public LocalizedTopic()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the localized topic identifier
        /// </summary>
        public int TopicLocalizedId { get; set; }

        /// <summary>
        /// Gets or sets the topic identifier
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }
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
        /// Gets the topic
        /// </summary>
        public Topic Topic
        {
            get
            {
                return IoC.Resolve<ITopicService>().GetTopicById(this.TopicId);
            }
        }
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the topic
        /// </summary>
        public virtual Topic NpTopic { get; set; }

        #endregion
    }

}
