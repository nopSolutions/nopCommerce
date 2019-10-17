using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a gift card mapping configuration
    /// </summary>
    public partial class GiftCardMap : NopEntityTypeConfiguration<GiftCard>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<GiftCard> builder)
        {
            builder.ToTable(nameof(GiftCard));
            builder.HasKey(giftCard => giftCard.Id);

            builder.Property(giftCard => giftCard.Amount).HasColumnType("decimal(18, 4)");

            builder.HasOne<OrderItem>().WithMany().HasForeignKey(giftCard => giftCard.PurchasedWithOrderItemId);

            builder.Ignore(giftCard => giftCard.GiftCardType);

            base.Configure(builder);
        }

        #endregion
    }
}