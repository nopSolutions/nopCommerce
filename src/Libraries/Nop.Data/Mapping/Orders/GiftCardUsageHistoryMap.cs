using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a gift card usage history mapping configuration
    /// </summary>
    public partial class GiftCardUsageHistoryMap : NopEntityTypeConfiguration<GiftCardUsageHistory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<GiftCardUsageHistory> builder)
        {
            builder.HasTableName(nameof(GiftCardUsageHistory));

            builder.Property(historyEntry => historyEntry.UsedValue).HasDbType("decimal(18, 4)");

            builder.Property(historyEntry => historyEntry.GiftCardId);
            builder.Property(historyEntry => historyEntry.UsedWithOrderId);
            builder.Property(historyEntry => historyEntry.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(historyEntry => historyEntry.GiftCard)
            //    .WithMany(giftCard => giftCard.GiftCardUsageHistory)
            //    .HasForeignKey(historyEntry => historyEntry.GiftCardId)
            //    .IsColumnRequired();

            //builder.HasOne(historyEntry => historyEntry.UsedWithOrder)
            //    .WithMany(order => order.GiftCardUsageHistory)
            //    .HasForeignKey(historyEntry => historyEntry.UsedWithOrderId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}