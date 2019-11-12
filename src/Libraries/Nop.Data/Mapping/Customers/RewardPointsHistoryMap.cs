using LinqToDB.Mapping;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Represents a reward points history mapping configuration
    /// </summary>
    public partial class RewardPointsHistoryMap : NopEntityTypeConfiguration<RewardPointsHistory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<RewardPointsHistory> builder)
        {
            builder.HasTableName(nameof(RewardPointsHistory));

            builder.Property(historyEntry => historyEntry.UsedAmount).HasDbType("decimal(18, 4)");

            builder.Property(historyEntry => historyEntry.CustomerId);
            builder.Property(historyEntry => historyEntry.StoreId);
            builder.Property(historyEntry => historyEntry.Points);
            builder.Property(historyEntry => historyEntry.PointsBalance);
            builder.Property(historyEntry => historyEntry.Message);
            builder.Property(historyEntry => historyEntry.CreatedOnUtc);
            builder.Property(historyEntry => historyEntry.EndDateUtc);
            builder.Property(historyEntry => historyEntry.ValidPoints);
            builder.Property(historyEntry => historyEntry.OrderId);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(historyEntry => historyEntry.Customer)
            //    .WithMany()
            //    .HasForeignKey(historyEntry => historyEntry.CustomerId)
            //    .IsColumnRequired();

            //builder.HasOne(historyEntry => historyEntry.UsedWithOrder)
            //    .WithOne(order => order.RedeemedRewardPointsEntry)
            //    .HasForeignKey<Order>(order => order.RewardPointsHistoryEntryId)
            //    .OnDelete(DeleteBehavior.ClientSetNull);
        }

        #endregion
    }
}