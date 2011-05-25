
using System;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer generated content
    /// </summary>
    public partial class CustomerContent : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the IP address
        /// </summary>
        public virtual string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is approved
        /// </summary>
        public virtual bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public virtual DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
    }
}
