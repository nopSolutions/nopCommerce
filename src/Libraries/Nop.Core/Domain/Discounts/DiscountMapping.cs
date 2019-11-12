namespace Nop.Core.Domain.Discounts
{
    public abstract partial class DiscountMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        public int DiscountId { get; set; }
    }
}
