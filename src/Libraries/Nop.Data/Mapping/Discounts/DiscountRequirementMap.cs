using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    public partial class DiscountRequirementMap : NopEntityTypeConfiguration<DiscountRequirement>
    {
        public DiscountRequirementMap()
        {
            this.ToTable("DiscountRequirement");
            this.HasKey(dr => dr.Id);

            this.Ignore(d => d.InteractionType);
            this.HasMany(dr => dr.ChildRequirements).WithOptional().HasForeignKey(dr => dr.ParentId);
        }
    }
}