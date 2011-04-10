using System;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Forums
{
    /// <summary>
    /// Represents a private message
    /// </summary>
    public partial class PrivateMessage : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user identifier who sent the message
        /// </summary>
        public int FromUserId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier who should receive the message
        /// </summary>
        public int ToUserId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is read
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is deleted by author
        /// </summary>
        public bool IsDeletedByAuthor { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is deleted by recipient
        /// </summary>
        public bool IsDeletedByRecipient { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets the user who sent the message
        /// </summary>
        public virtual Customer FromCustomer { get; set; }

        /// <summary>
        /// Gets the user who should receive the message
        /// </summary>
        public virtual Customer ToCustomer { get; set; }

    }
}
