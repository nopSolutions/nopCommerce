using LinqToDB.Mapping;
using System;
using Nop.Core.Data;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum subscription item
    /// </summary>
    [Table(NopMappingDefaults.ForumsSubscriptionTable)]
    public partial class ForumSubscription : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum subscription identifier
        /// </summary>
        [Column]
        public Guid SubscriptionGuid { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        [Column]
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the forum identifier
        /// </summary>
        [Column]
        public int ForumId { get; set; }

        /// <summary>
        /// Gets or sets the topic identifier
        /// </summary>
        [Column]
        public int TopicId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        [Column]
        public DateTime CreatedOnUtc { get; set; }
    }
}
