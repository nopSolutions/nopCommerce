using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class DiscountMap : NopEntityTypeConfiguration<Discount>
    {
        public override void Configure(EntityTypeBuilder<Discount> builder)
        {
            base.Configure(builder);
            builder.ToTable("Discount");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(200);
            builder.Property(d => d.CouponCode).HasMaxLength(100);
            builder.Property(d => d.DiscountPercentage);
            builder.Property(d => d.DiscountAmount);
            builder.Property(d => d.MaximumDiscountAmount);

            builder.Ignore(d => d.DiscountType);
            builder.Ignore(d => d.DiscountLimitation);

            builder.HasMany(dr => dr.DiscountRequirements)
                .WithOne(d => d.Discount)
                .IsRequired(true)
                .HasForeignKey(dr => dr.DiscountId);
        }
    }
}