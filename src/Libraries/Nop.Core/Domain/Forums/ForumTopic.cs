using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum topic
    /// </summary>
    public partial class ForumTopic : BaseEntity
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ForumTopic()
        {
            this.ForumPosts = new List<ForumPost>();
        }

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

        /// <summary>
        /// Gets or sets the forum topic type
        /// </summary>
        public ForumTopicTypeEnum ForumTopicType
        {
            get
            {
                return (ForumTopicTypeEnum)this.TopicTypeId;
            }
            set
            {
                this.TopicTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets the forum
        /// </summary>
        public virtual Forum Forum { get; set; }

        /// <summary>
        /// Gets the user
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// The posts in the topic
        /// </summary>
        public virtual ICollection<ForumPost> ForumPosts { get; set; }

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
        /// Gets the first ForumPost
        /// </summary>
        public ForumPost FirstPost
        {
            get
            {
                if (this.ForumPosts.Count > 0)
                {
                    return this.ForumPosts.OrderBy(fp => fp.CreatedOn).ThenBy(fp => fp.Id).FirstOrDefault();
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
                if (this.ForumPosts.Count > 0)
                {
                    return this.ForumPosts.OrderBy(fp => fp.CreatedOn).ThenBy(fp => fp.Id).LastOrDefault();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the last post user
        /// </summary>
        public Customer LastPostUser
        {
            get
            {
                if (this.LastPost != null)
                {
                    return this.LastPost.Customer;
                }
                return null;
            }
        }

    }
}
