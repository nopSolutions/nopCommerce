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
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;


namespace NopSolutions.NopCommerce.BusinessLogic.Content.Forums
{
    /// <summary>
    /// Represents a forum subscription item
    /// </summary>
    public partial class ForumSubscription : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ForumSubscription class
        /// </summary>
        public ForumSubscription()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the forum subscription identifier
        /// </summary>
        public int ForumSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the forum subscription identifier
        /// </summary>
        public Guid SubscriptionGuid { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the forum identifier
        /// </summary>
        public int ForumId { get; set; }

        /// <summary>
        /// Gets or sets the topic identifier
        /// </summary>
        public int TopicId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the user
        /// </summary>
        public Customer User
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetCustomerById(this.UserId);
            }
        }
        
        /// <summary>
        /// Gets the forum
        /// </summary>
        public Forum Forum
        {
            get
            {
                return IoC.Resolve<IForumService>().GetForumById(this.ForumId);
            }
        }

        /// <summary>
        /// Gets the topic
        /// </summary>
        public ForumTopic Topic
        {
            get
            {
                return IoC.Resolve<IForumService>().GetTopicById(this.TopicId);
            }
        }

        #endregion
    }
}