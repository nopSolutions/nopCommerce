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
        /// Gets or sets the entity identifier
        /// </summary>
        [NotColumn]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        [Column("Category_Id")]
        public int CategoryId { get; set; }
    }
}