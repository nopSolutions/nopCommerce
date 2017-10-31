
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class GiftCardUsageHistoryMap : NopEntityTypeConfiguration<GiftCardUsageHistory>
    {
        public override void Configure(EntityTypeBuilder<GiftCardUsageHistory> builder)
        {
            base.Configure(builder);
            builder.ToTable("GiftCardUsageHistory");
            builder.HasKey(gcuh => gcuh.Id);
            builder.Property(gcuh => gcuh.UsedValue);
            //builder.Property(gcuh => gcuh.UsedValueInCustomerCurrency).HasPrecision(18, 4);

            builder.HasOne(gcuh => gcuh.GiftCard)
                .WithMany(gc => gc.GiftCardUsageHistory)
                .IsRequired(true)
                .HasForeignKey(gcuh => gcuh.GiftCardId);

            builder.HasOne(gcuh => gcuh.UsedWithOrder)
                .WithMany(o => o.GiftCardUsageHistory)
                .IsRequired(true)
                .HasForeignKey(gcuh => gcuh.UsedWithOrderId);
        }
    }
}