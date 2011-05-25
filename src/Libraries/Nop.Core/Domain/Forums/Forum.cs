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
        private ICollection<ForumTopic> _forumTopics;

        /// <summary>
        /// Gets or sets the forum group identifier
        /// </summary>
        public virtual int ForumGroupId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the number of topics
        /// </summary>
        public virtual int NumTopics { get; set; }

        /// <summary>
        /// Gets or sets the number of posts
        /// </summary>
        public virtual int NumPosts { get; set; }

        /// <summary>
        /// Gets or sets the last topic identifier
        /// </summary>
        public virtual int LastTopicId { get; set; }

        /// <summary>
        /// Gets or sets the last post identifier
        /// </summary>
        public virtual int LastPostId { get; set; }

        /// <summary>
        /// Gets or sets the last post customer identifier
        /// </summary>
        public virtual int LastPostCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the last post date and time
        /// </summary>
        public virtual DateTime? LastPostTime { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public virtual DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the collection of Forums
        /// </summary>
        public virtual ICollection<ForumTopic> ForumTopics
        {
            get { return _forumTopics ?? (_forumTopics = new List<ForumTopic>()); }
            protected set { _forumTopics = value; }
        }
        /// <summary>
        /// Gets the ForumGroup
        /// </summary>
        public virtual ForumGroup ForumGroup { get; set; }

        /// <summary>
        /// Gets the last topic
        /// </summary>
        public virtual ForumTopic LastTopic
        {
            get
            {
                if (this.ForumTopics.Count > 0)
                {
                    return this.ForumTopics.OrderBy(ft => ft.CreatedOnUtc).ThenBy(ft => ft.Id).LastOrDefault();
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last post
        /// </summary>
        public virtual ForumPost LastPost
        {
            get
            {
                var lastTopic = this.LastTopic;
                if (lastTopic != null)
                {
                    return lastTopic.ForumPosts.OrderBy(fp => fp.CreatedOnUtc).ThenBy(fp => fp.Id).LastOrDefault();
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last post customer
        /// </summary>
        public virtual Customer LastPostCustomer
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
