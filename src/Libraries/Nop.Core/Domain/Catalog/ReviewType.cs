using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a review type
    /// </summary>
    public partial class ReviewType : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the review type is visible to all customers
        /// </summary>
        public bool VisibleToAllCustomers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the review type is required
        /// </summary>
        public bool IsRequired { get; set; }
    }
}
