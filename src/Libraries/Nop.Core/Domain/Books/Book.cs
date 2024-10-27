using System;

namespace Nop.Core.Domain.Books
{
    /// <summary>
    /// Represent a book
    /// </summary>
    public partial class Book : BaseEntity
    {
        /// <summary>
        /// Gets or Sets book name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
