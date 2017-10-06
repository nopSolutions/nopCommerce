using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    public partial class DiscountUsageHistoryMap : NopEntityTypeConfiguration<DiscountUsageHistory>
    {
        public override void Configure(EntityTypeBuilder<DiscountUsageHistory> builder)
        {
            base.Configure(builder);
            builder.ToTable("DiscountUsageHistory");
            builder.HasKey(duh => duh.Id);

            builder.HasOne(duh => duh.Discount)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(duh => duh.DiscountId);

            builder.HasOne(duh => duh.Order)
                .WithMany(o => o.DiscountUsageHistory)
                .IsRequired(true)
                .HasForeignKey(duh => duh.OrderId);
        }
    }
}