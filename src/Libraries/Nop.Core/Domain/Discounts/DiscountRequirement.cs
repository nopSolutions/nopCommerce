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
        public virtual int DiscountId { get; set; }
        
        /// <summary>
        /// Gets or sets the discount requirement rule system name
        /// </summary>
        public virtual string DiscountRequirementRuleSystemName { get; set; }

        /// <summary>
        /// Gets or sets the the discount requirement spent amount - customer had spent/purchased x.xx amount (used when requirement is set to "Customer had spent/purchased x.xx amount")
        /// </summary>
        public virtual decimal SpentAmount { get; set; }
        
        /// <summary>
        /// Gets or sets the restricted product variant identifiers (comma separated)
        /// </summary>
        public virtual string RestrictedProductVariantIds { get; set; }
        
        /// <summary>
        /// Gets or sets the discount
        /// </summary>
        public virtual Discount Discount { get; set; }

    }
}
