using LinqToDB.Mapping;
using System;
using Nop.Core.Data;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a private message
    /// </summary>
    [Table(NopMappingDefaults.PrivateMessageTable)]
    public partial class PrivateMessage : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        [Column]
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier who sent the message
        /// </summary>
        [Column]
        public int FromCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier who should receive the message
        /// </summary>
        [Column]
        public int ToCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [Column]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        [Column]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether message is read
        /// </summary>
        [Column]
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether message is deleted by author
        /// </summary>
        [Column]
        public bool IsDeletedByAuthor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether message is deleted by recipient
        /// </summary>
        [Column]
        public bool IsDeletedByRecipient { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        [Column]
        public DateTime CreatedOnUtc { get; set; }
    }
}
