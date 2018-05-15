using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class DiscountRequirementMap : NopEntityTypeConfiguration<DiscountRequirement>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public DiscountRequirementMap()
        {
            this.ToTable("DiscountRequirement");
            this.HasKey(requirement => requirement.Id);

            this.Ignore(requirement => requirement.InteractionType);
            this.HasMany(requirement => requirement.ChildRequirements)
                .WithOptional()
                .HasForeignKey(requirement => requirement.ParentId);
        }
    }
}