using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    public partial class DiscountRequirementMap : NopEntityTypeConfiguration<DiscountRequirement>
    {
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