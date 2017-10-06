using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    public partial class DiscountRequirementMap : NopEntityTypeConfiguration<DiscountRequirement>
    {
        public override void Configure(EntityTypeBuilder<DiscountRequirement> builder)
        {
            base.Configure(builder);
            builder.ToTable("DiscountRequirement");
            builder.HasKey(requirement => requirement.Id);

            builder.Ignore(requirement => requirement.InteractionType);
            builder.HasMany(requirement => requirement.ChildRequirements)
                .WithOne()
                .HasForeignKey(requirement => requirement.ParentId);
        }
    }
}