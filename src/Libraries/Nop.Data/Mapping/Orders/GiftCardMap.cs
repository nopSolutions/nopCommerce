using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class GiftCardMap : NopEntityTypeConfiguration<GiftCard>
    {
        public override void Configure(EntityTypeBuilder<GiftCard> builder)
        {
            base.Configure(builder);
            builder.ToTable("GiftCard");
            builder.HasKey(gc => gc.Id);

            builder.Property(gc => gc.Amount);

            builder.Ignore(gc => gc.GiftCardType);

            builder.HasOne(gc => gc.PurchasedWithOrderItem)
                .WithMany(orderItem => orderItem.AssociatedGiftCards)
                .HasForeignKey(gc => gc.PurchasedWithOrderItemId);
        }
    }
}