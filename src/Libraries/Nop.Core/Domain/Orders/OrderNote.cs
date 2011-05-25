using System;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order note
    /// </summary>
    public partial class OrderNote : BaseEntity
    {
        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public virtual int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the note
        /// </summary>
        public virtual string Note { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a customer can see a note
        /// </summary>
        public virtual bool DisplayToCustomer { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order note creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the order
        /// </summary>
        public virtual Order Order { get; set; }
    }

}
