namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount-category mapping class
    /// </summary>
    public partial class DiscountCategoryMapping : DiscountMapping
    {
        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int CategoryId { get; set; }
    }
}