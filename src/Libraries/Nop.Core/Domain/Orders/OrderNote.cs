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
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the attached file (download) identifier
        /// </summary>
        public int DownloadId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a customer can see a note
        /// </summary>
        public bool DisplayToCustomer { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order note creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the order
        /// </summary>
        public virtual Order Order { get; set; }
    }
}
