using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a recurring payment history mapping configuration
    /// </summary>
    public partial class RecurringPaymentHistoryMap : NopEntityTypeConfiguration<RecurringPaymentHistory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<RecurringPaymentHistory> builder)
        {
            builder.HasTableName(nameof(RecurringPaymentHistory));

            builder.Property(historyEntry => historyEntry.RecurringPaymentId);
            builder.Property(historyEntry => historyEntry.OrderId);
            builder.Property(historyEntry => historyEntry.CreatedOnUtc);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(historyEntry => historyEntry.RecurringPayment)
            //    .WithMany(recurringPayment => recurringPayment.RecurringPaymentHistory)
            //    .HasForeignKey(historyEntry => historyEntry.RecurringPaymentId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}