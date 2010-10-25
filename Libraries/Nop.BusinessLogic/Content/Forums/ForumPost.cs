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
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Content.Forums
{
    /// <summary>
    /// Represents a forum post
    /// </summary>
    public partial class ForumPost : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ForumPost class
        /// </summary>
        public ForumPost()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the forum post identifier
        /// </summary>
        public int ForumPostId { get; set; }

        /// <summary>
        /// Gets or sets the forum topic identifier
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        #endregion
        
        #region Custom Properties

        /// <summary>
        /// Gets the topic
        /// </summary>
        public ForumTopic Topic
        {
            get
            {
                return IoCFactory.Resolve<IForumManager>().GetTopicById(this.TopicId);
            }
        }

        /// <summary>
        /// Gets the user
        /// </summary>
        public Customer User
        {
            get
            {
                return IoCFactory.Resolve<ICustomerManager>().GetCustomerById(this.UserId);
            }
        }
        #endregion
    }
}