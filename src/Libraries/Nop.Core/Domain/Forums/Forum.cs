using LinqToDB.Mapping;
using System;
using Nop.Core.Data;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum
    /// </summary>
    [Table(NopMappingDefaults.ForumTable)]
    public partial class Forum : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum group identifier
        /// </summary>
        [Column]
        public int ForumGroupId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [Column]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [Column]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the number of topics
        /// </summary>
        [Column]
        public int NumTopics { get; set; }

        /// <summary>
        /// Gets or sets the number of posts
        /// </summary>
        [Column]
        public int NumPosts { get; set; }

        /// <summary>
        /// Gets or sets the last topic identifier
        /// </summary>
        [Column]
        public int LastTopicId { get; set; }

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
        /// Gets or sets the display order
        /// </summary>
        [Column]
        public int DisplayOrder { get; set; }

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
    }
}
