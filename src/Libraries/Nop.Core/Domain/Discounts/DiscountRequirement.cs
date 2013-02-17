namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount requirement
    /// </summary>
    public partial class DiscountRequirement : BaseEntity
    {
        /// <summary>
        /// Gets or sets the discount identifier
        /// </summary>
        public int DiscountId { get; set; }
        
        /// <summary>
        /// Gets or sets the discount requirement rule system name
        /// </summary>
        public string DiscountRequirementRuleSystemName { get; set; }
        
        /// <summary>
        /// Gets or sets the discount
        /// </summary>
        public virtual Discount Discount { get; set; }

    }
}
