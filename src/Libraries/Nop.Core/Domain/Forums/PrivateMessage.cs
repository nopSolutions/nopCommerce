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
        /// Gets or sets the customer identifier who sent the message
        /// </summary>
        public virtual int FromCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier who should receive the message
        /// </summary>
        public virtual int ToCustomerId { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is read
        /// </summary>
        public virtual bool IsRead { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is deleted by author
        /// </summary>
        public virtual bool IsDeletedByAuthor { get; set; }

        /// <summary>
        /// Gets or sets a value indivating whether message is deleted by recipient
        /// </summary>
        public virtual bool IsDeletedByRecipient { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the customer who sent the message
        /// </summary>
        public virtual Customer FromCustomer { get; set; }

        /// <summary>
        /// Gets the customer who should receive the message
        /// </summary>
        public virtual Customer ToCustomer { get; set; }
    }
}
