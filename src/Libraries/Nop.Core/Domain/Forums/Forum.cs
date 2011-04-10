using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum
    /// </summary>
    public partial class Forum : BaseEntity
    {
        /// <summary>
        /// ctor
        /// </summary>
        public Forum()
        {
            this.ForumTopics = new List<ForumTopic>();
        }

        /// <summary>
        /// Gets or sets the forum group identifier
        /// </summary>
        public int ForumGroupId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the number of topics
        /// </summary>
        public int NumTopics { get; set; }

        /// <summary>
        /// Gets or sets the number of posts
        /// </summary>
        public int NumPosts { get; set; }

        /// <summary>
        /// Gets or sets the last topic identifier
        /// </summary>
        public int LastTopicId { get; set; }

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
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the collection of Forums
        /// </summary>
        public virtual ICollection<ForumTopic> ForumTopics { get; set; }

        /// <summary>
        /// Gets the ForumGroup
        /// </summary>
        public virtual ForumGroup ForumGroup { get; set; }

        /// <summary>
        /// Gets the last topic
        /// </summary>
        public ForumTopic LastTopic
        {
            get
            {
                if (this.ForumTopics.Count > 0)
                {
                    return this.ForumTopics.OrderBy(ft => ft.CreatedOn).ThenBy(ft => ft.Id).LastOrDefault();
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
                var lastTopic = this.LastTopic;
                if (lastTopic != null)
                {
                    return lastTopic.ForumPosts.OrderBy(fp => fp.CreatedOn).ThenBy(fp => fp.Id).LastOrDefault();
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
                var lastPost = this.LastPost;
                if (lastPost != null)
                {
                    return lastPost.Customer;
                }
                return null;
            }
        }
    }
}
