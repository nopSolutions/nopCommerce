using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount-category mapping class
    /// </summary>
    [Table(NopMappingDefaults.DiscountAppliedToCategoriesTable)]
    public partial class DiscountCategoryMapping : DiscountMapping
    {
        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        [Column]
        public int CategoryId { get; set; }
    }
}