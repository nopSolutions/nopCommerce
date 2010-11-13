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
    /// Represents a forum topic
    /// </summary>
    public partial class ForumTopic : BaseEntity
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the ForumTopic class
        /// </summary>
        public ForumTopic()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the forum topic identifier
        /// </summary>
        public int ForumTopicId { get; set; }

        /// <summary>
        /// Gets or sets the forum identifier
        /// </summary>
        public int ForumId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the topic type identifier
        /// </summary>
        public int TopicTypeId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the number of posts
        /// </summary>
        public int NumPosts { get; set; }

        /// <summary>
        /// Gets or sets the number of views
        /// </summary>
        public int Views { get; set; }

        /// <summary>
        /// Gets or sets the last post identifier
        /// </summary>
        public int LastPostId { get; set; }

        /// <summary>
        /// Gets or sets the last post user identifier
        /// </summary>
        public int LastPostUserId { get; set; }

        /// <summary>
        /// Gets or sets the last post date and time
        /// </summary>
        public DateTime? LastPostTime { get; set; }

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
        /// Gets the number of replies
        /// </summary>
        public int NumReplies
        {
            get
            {
                int result = 0;
                if (NumPosts > 0)
                    result = NumPosts - 1;
                return result;
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
        /// Gets the log type
        /// </summary>
        public ForumTopicTypeEnum TopicType
        {
            get
            {
                return (ForumTopicTypeEnum)this.TopicTypeId;
            }
        }

        /// <summary>
        /// Gets the first post
        /// </summary>
        public ForumPost FirstPost
        {
            get
            {
                var forumPosts = IoC.Resolve<IForumService>().GetAllPosts(this.ForumTopicId, 
                    0, string.Empty, 0, 1);
                if (forumPosts.Count > 0)
                {
                    return forumPosts[0];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the last post
        /// </summary>
        public ForumPost LastPost
        {
            get
            {
                return IoC.Resolve<IForumService>().GetPostById(this.LastPostId);
            }
        }

        /// <summary>
        /// Gets the last post user
        /// </summary>
        public Customer LastPostUser
        {
            get
            {
                return IoC.Resolve<ICustomerService>().GetCustomerById(this.LastPostUserId);
            }
        }

        #endregion
    }
}