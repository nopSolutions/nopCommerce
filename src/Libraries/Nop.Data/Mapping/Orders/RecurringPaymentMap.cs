using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a recurring payment mapping configuration
    /// </summary>
    public partial class RecurringPaymentMap : NopEntityTypeConfiguration<RecurringPayment>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<RecurringPayment> builder)
        {
            builder.HasTableName(nameof(RecurringPayment));

            builder.Property(recurringPayment => recurringPayment.CycleLength);
            builder.Property(recurringPayment => recurringPayment.CyclePeriodId);
            builder.Property(recurringPayment => recurringPayment.TotalCycles);
            builder.Property(recurringPayment => recurringPayment.StartDateUtc);
            builder.Property(recurringPayment => recurringPayment.IsActive);
            builder.Property(recurringPayment => recurringPayment.LastPaymentFailed);
            builder.Property(recurringPayment => recurringPayment.Deleted);
            builder.Property(recurringPayment => recurringPayment.InitialOrderId);
            builder.Property(recurringPayment => recurringPayment.CreatedOnUtc);

            builder.Ignore(recurringPayment => recurringPayment.CyclePeriod);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(recurringPayment => recurringPayment.InitialOrder)
            //    .WithMany()
            //    .HasForeignKey(recurringPayment => recurringPayment.InitialOrderId)
            //    .IsColumnRequired()
            //    .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion
    }
}