using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount-product mapping class
    /// </summary>
    [Table(NopMappingDefaults.DiscountAppliedToProductsTable)]
    public partial class DiscountProductMapping : DiscountMapping
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [Column("Product_Id")]
        public int ProductId { get; set; }
    }
}