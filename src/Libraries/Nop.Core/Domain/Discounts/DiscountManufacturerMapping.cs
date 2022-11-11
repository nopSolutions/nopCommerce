namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount-manufacturer mapping class
    /// </summary>
    public partial class DiscountManufacturerMapping : DiscountMapping
    {
        /// <summary>
        /// Gets or sets the manufacturer identifier
        /// </summary>
        public override int EntityId { get; set; }
    }
}