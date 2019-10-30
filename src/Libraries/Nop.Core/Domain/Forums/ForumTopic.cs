using LinqToDB.Mapping;
using System;
using Nop.Core.Data;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum topic
    /// </summary>
    [Table(NopMappingDefaults.ForumsTopicTable)]
    public partial class ForumTopic : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum identifier
        /// </summary>
        [Column]
        public int ForumId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        [Column]
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the topic type identifier
        /// </summary>
        [Column]
        public int TopicTypeId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [Column]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the number of posts
        /// </summary>
        [Column]
        public int NumPosts { get; set; }

        /// <summary>
        /// Gets or sets the number of views
        /// </summary>
        [Column]
        public int Views { get; set; }

        /// <summary>
        /// Gets or sets the last post identifier
        /// </summary>
        [Column]
        public int LastPostId { get; set; }

        /// <summary>
        /// Gets or sets the last post customer identifier
        /// </summary>
        [Column]
        public int LastPostCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the last post date and time
        /// </summary>
        [Column]
        public DateTime? LastPostTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        [Column]
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        [Column]
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the forum topic type
        /// </summary>
        [NotColumn]
        public ForumTopicType ForumTopicType
        {
            get => (ForumTopicType)TopicTypeId;
            set => TopicTypeId = (int)value;
        }

        /// <summary>
        /// Gets the number of replies
        /// </summary>
        [NotColumn]
        public int NumReplies
        {
            get
            {
                var result = 0;

                if (NumPosts > 0)
                    result = NumPosts - 1;

                return result;
            }
        }
    }
}
