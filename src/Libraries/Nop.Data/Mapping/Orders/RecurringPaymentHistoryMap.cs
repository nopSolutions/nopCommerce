using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        public override void Configure(EntityTypeBuilder<RecurringPaymentHistory> builder)
        {
            builder.ToTable(nameof(RecurringPaymentHistory));
            builder.HasKey(historyEntry => historyEntry.Id);

            builder.HasOne(historyEntry => historyEntry.RecurringPayment)
                .WithMany(recurringPayment => recurringPayment.RecurringPaymentHistory)
                .HasForeignKey(historyEntry => historyEntry.RecurringPaymentId)
                .IsRequired();

            base.Configure(builder);
        }

        #endregion
    }
}