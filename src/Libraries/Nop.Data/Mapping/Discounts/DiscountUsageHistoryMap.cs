using LinqToDB.Mapping;
using Nop.Core.Domain.Discounts;

namespace Nop.Data.Mapping.Discounts
{
    /// <summary>
    /// Represents a discount usage history mapping configuration
    /// </summary>
    public partial class DiscountUsageHistoryMap : NopEntityTypeConfiguration<DiscountUsageHistory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<DiscountUsageHistory> builder)
        {
            builder.HasTableName(nameof(DiscountUsageHistory));

            builder.Property(historyEntry => historyEntry.DiscountId);
            builder.Property(historyEntry => historyEntry.OrderId);
            builder.Property(historyEntry => historyEntry.CreatedOnUtc);
        }

        #endregion
    }
}