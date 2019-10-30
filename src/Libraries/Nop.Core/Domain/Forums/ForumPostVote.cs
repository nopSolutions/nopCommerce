using LinqToDB.Mapping;
using System;
using Nop.Core.Data;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum post vote
    /// </summary>
    [Table(NopMappingDefaults.ForumsPostVoteTable)]
    public partial class ForumPostVote : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum post identifier
        /// </summary>
        [Column]
        public int ForumPostId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        [Column]
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this vote is up or is down
        /// </summary>
        [Column]
        public bool IsUp { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        [Column]
        public DateTime CreatedOnUtc { get; set; }
    }
}
