using LinqToDB.Mapping;
using System;
using Nop.Core.Data;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum post
    /// </summary>
    [Table(NopMappingDefaults.ForumsPostTable)]
    public partial class ForumPost : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum topic identifier
        /// </summary>
        [Column]
        public int TopicId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        [Column]
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        [Column]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        [Column]
        public string IPAddress { get; set; }

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
        /// Gets or sets the count of votes
        /// </summary>
        [Column]
        public int VoteCount { get; set; }
    }
}
