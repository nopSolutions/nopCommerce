using System;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum topic
    /// </summary>
    public partial class ForumTopic : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum identifier
        /// </summary>
        public virtual int ForumId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the topic type identifier
        /// </summary>
        public virtual int TopicTypeId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets or sets the number of posts
        /// </summary>
        public virtual int NumPosts { get; set; }

        /// <summary>
        /// Gets or sets the number of views
        /// </summary>
        public virtual int Views { get; set; }

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
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public virtual DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the forum topic type
        /// </summary>
        public virtual ForumTopicType ForumTopicType
        {
            get
            {
                return (ForumTopicType)this.TopicTypeId;
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
        /// Gets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets the number of replies
        /// </summary>
        public virtual int NumReplies
        {
            get
            {
                int result = 0;
                if (NumPosts > 0)
                    result = NumPosts - 1;
                return result;
            }
        }
    }
}
