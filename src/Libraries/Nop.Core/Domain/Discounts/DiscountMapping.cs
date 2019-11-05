using LinqToDB.Mapping;

namespace Nop.Core.Domain.Discounts
{
    public abstract partial class DiscountMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [NotColumn]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        [Column("Discount_Id")]
        public int DiscountId { get; set; }
    }
}
